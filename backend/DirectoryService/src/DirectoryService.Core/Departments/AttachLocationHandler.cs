using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий привязки локации к подразделению: проверяет, что обе стороны существуют и связи ещё нет, и создаёт связь.
/// Не бросает; успех логируется как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class AttachLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<AttachLocationHandler> logger)
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<AttachLocationHandler> _logger = logger;

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
        {
            _logger.LogWarning(
                "Location {LocationId} is already attached to department {DepartmentId}.",
                locationId,
                departmentId);

            return Failure.From(Error.Conflict(
                $"Локация '{locationId}' уже привязана к подразделению '{departmentId}'.",
                code: "directory.department_location.conflict"));
        }

        // «Связь не найдена» — ожидаемый путь; настоящую ошибку БД пробрасываем наверх.
        if (existingLinkResult.Error.Any(error => error.Type != ErrorType.NotFound))
            return existingLinkResult.Error;

        var link = DepartmentLocation.Create(typedDepartmentId, typedLocationId, isPrimary: false);

        UnitResult<Failure> addResult = _departmentsRepository.AddDepartmentLocation(link);
        if (addResult.IsFailure)
            return addResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Location {LocationId} attached to department {DepartmentId}.",
            locationId,
            departmentId);

        return UnitResult.Success<Failure>();
    }
}
