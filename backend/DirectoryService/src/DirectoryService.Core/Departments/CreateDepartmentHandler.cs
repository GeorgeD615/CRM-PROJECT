using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments.Exceptions;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий создания подразделения: проверяет существование родителя и локаций,
/// строит доменную модель с путём из slug'ов и атомарно сохраняет подразделение
/// вместе с его первичными связями с локациями.
/// </summary>
public sealed class CreateDepartmentHandler(
    IValidator<CreateDepartmentRequest> validator,
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<CreateDepartmentHandler> logger)
{
    private readonly IValidator<CreateDepartmentRequest> _validator = validator;
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<CreateDepartmentHandler> _logger = logger;

    public async Task<Guid> HandleAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new RequestValidationException(validationResult.ToErrors());

        var name = DepartmentName.Create(request.Name);
        var slug = DepartmentSlug.Create(request.Slug);

        IReadOnlyCollection<LocationId> locationIds = [.. request.LocationIds
            .Distinct()
            .Select(LocationId.Create)];

        await EnsureLocationsExistAsync(locationIds, cancellationToken);

        Department department = request.ParentId is { } parentId
            ? Department.CreateChild(name, slug, await GetParentAsync(parentId, cancellationToken))
            : Department.CreateRoot(name, slug);

        DepartmentLocation[] departmentLocations = [.. locationIds.Select(locationId =>
            DepartmentLocation.Create(department.Id, locationId, isPrimary: false))];

        _departmentsRepository.Add(department, departmentLocations);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Department {DepartmentId} created with path {DepartmentPath} and {LocationCount} locations.",
            department.Id.Value,
            department.Path.Value,
            departmentLocations.Length);

        return department.Id.Value;
    }

    private async Task EnsureLocationsExistAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        if (locationIds.Count == 0)
            return;

        IReadOnlyCollection<LocationId> existingIds =
            await _locationsRepository.GetExistingIdsAsync(locationIds, cancellationToken);

        Guid[] missingIds = [.. locationIds
            .Except(existingIds)
            .Select(id => id.Value)];

        if (missingIds.Length == 0)
            return;

        throw new LocationsNotFoundException(missingIds);
    }

    private async Task<Department> GetParentAsync(Guid parentId, CancellationToken cancellationToken)
    {
        Department? parent = await _departmentsRepository.GetByIdAsync(
            DepartmentId.Create(parentId),
            cancellationToken);

        return parent is null ? throw new ParentDepartmentNotFoundException(parentId) : parent;
    }
}
