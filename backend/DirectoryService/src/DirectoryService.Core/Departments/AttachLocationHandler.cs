using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий привязки локации к подразделению: проверяет, что обе стороны существуют и связи ещё нет, и создаёт связь.
/// Не бросает и не логирует — все ошибки возвращаются как результат.
/// </summary>
public sealed class AttachLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager)
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<UnitResult<Failure>> HandleAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var typedDepartmentId = DepartmentId.Create(departmentId);
        var typedLocationId = LocationId.Create(locationId);

        Result<Department, Failure> departmentResult =
            await _departmentsRepository.GetByIdAsync(typedDepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        Result<Location, Failure> locationResult =
            await _locationsRepository.GetByIdAsync(typedLocationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        Result<DepartmentLocation, Failure> existingLinkResult = await _departmentsRepository.GetDepartmentLocationAsync(
            typedDepartmentId,
            typedLocationId,
            cancellationToken);

        if (existingLinkResult.IsSuccess)
            return Failure.From(Error.Conflict(
                $"Локация '{locationId}' уже привязана к подразделению '{departmentId}'.",
                code: "directory.department_location.conflict"));

        // «Связь не найдена» — ожидаемый путь; настоящую ошибку БД пробрасываем наверх.
        if (existingLinkResult.Error.Any(error => error.Type != ErrorType.NotFound))
            return existingLinkResult.Error;

        var link = DepartmentLocation.Create(typedDepartmentId, typedLocationId, isPrimary: false);

        UnitResult<Failure> addResult = _departmentsRepository.AddDepartmentLocation(link);
        if (addResult.IsFailure)
            return addResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Failure>();
    }
}
