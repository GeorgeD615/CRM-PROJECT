using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий создания подразделения: проверяет существование родителя и локаций,
/// строит доменную модель с путём из slug'ов и атомарно сохраняет подразделение
/// вместе с его первичными связями с локациями. Не бросает и не логирует —
/// все ошибки возвращаются как результат.
/// </summary>
public sealed class CreateDepartmentHandler(
    IValidator<CreateDepartmentRequest> validator,
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager)
{
    private readonly IValidator<CreateDepartmentRequest> _validator = validator;
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<Result<Guid, Failure>> HandleAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var name = DepartmentName.Create(request.Name);
        var slug = DepartmentSlug.Create(request.Slug);

        IReadOnlyCollection<LocationId> locationIds = [.. request.LocationIds
            .Distinct()
            .Select(LocationId.Create)];

        UnitResult<Failure> ensureLocationsResult = await EnsureLocationsExistAsync(locationIds, cancellationToken);
        if (ensureLocationsResult.IsFailure)
            return ensureLocationsResult.Error;

        Result<Department, Failure> departmentResult;
        if (request.ParentId is { } parentId)
        {
            Result<Department, Failure> parentResult = await GetParentAsync(parentId, cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Error;

            departmentResult = Department.CreateChild(name, slug, parentResult.Value);
        }
        else
        {
            departmentResult = Department.CreateRoot(name, slug);
        }

        if (departmentResult.IsFailure)
            return departmentResult.Error;

        Department department = departmentResult.Value;

        DepartmentLocation[] departmentLocations = [.. locationIds.Select(locationId =>
            DepartmentLocation.Create(department.Id, locationId, isPrimary: false))];

        UnitResult<Failure> addResult = _departmentsRepository.Add(department, departmentLocations);
        if (addResult.IsFailure)
            return addResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return department.Id.Value;
    }

    private async Task<UnitResult<Failure>> EnsureLocationsExistAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        if (locationIds.Count == 0)
            return UnitResult.Success<Failure>();

        Result<IReadOnlyCollection<LocationId>, Failure> existingIdsResult =
            await _locationsRepository.GetExistingIdsAsync(locationIds, cancellationToken);
        if (existingIdsResult.IsFailure)
            return existingIdsResult.Error;

        Guid[] missingIds = [.. locationIds
            .Except(existingIdsResult.Value)
            .Select(id => id.Value)];

        if (missingIds.Length == 0)
            return UnitResult.Success<Failure>();

        Error[] errors = [.. missingIds.Select(locationId => Error.NotFound(
            $"Локация '{locationId}' не найдена.",
            code: "directory.location.not_found"))];

        return new Failure(errors);
    }

    private async Task<Result<Department, Failure>> GetParentAsync(Guid parentId, CancellationToken cancellationToken)
    {
        Result<Department, Failure> parentResult =
            await _departmentsRepository.GetByIdAsync(DepartmentId.Create(parentId), cancellationToken);

        if (parentResult.IsSuccess)
            return parentResult;

        // Инфраструктурную ошибку пробрасываем как есть; «не найдено» уточняем до родителя.
        if (parentResult.Error.Any(error => error.Type != ErrorType.NotFound))
            return parentResult;

        return Failure.From(Error.NotFound(
            $"Родительское подразделение '{parentId}' не найдено.",
            code: "directory.department.parent_not_found"));
    }
}
