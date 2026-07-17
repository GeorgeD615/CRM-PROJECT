using DirectoryService.Core.Database;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий привязки локации к подразделению: проверяет, что обе стороны существуют и связи ещё нет, и создаёт связь.
/// </summary>
public sealed class AttachLocationHandler
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AttachLocationHandler> _logger;

    public AttachLocationHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        ILogger<AttachLocationHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task HandleAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        (DepartmentId typedDepartmentId, LocationId typedLocationId) =
            await EnsureDepartmentAndLocationExistAsync(departmentId, locationId, cancellationToken);

        DepartmentLocation? existingLink = await _departmentsRepository.GetDepartmentLocationAsync(
            typedDepartmentId,
            typedLocationId,
            cancellationToken);

        if (existingLink is not null)
        {
            _logger.LogWarning(
                "Location {LocationId} is already attached to department {DepartmentId}.",
                locationId,
                departmentId);

            throw new DepartmentLocationAlreadyExistsException(departmentId, locationId);
        }

        var link = DepartmentLocation.Create(typedDepartmentId, typedLocationId, isPrimary: false);

        _departmentsRepository.AddDepartmentLocation(link);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Location {LocationId} attached to department {DepartmentId}.",
            locationId,
            departmentId);
    }

    private async Task<(DepartmentId DepartmentId, LocationId LocationId)> EnsureDepartmentAndLocationExistAsync(
        Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        var typedDepartmentId = DepartmentId.Create(departmentId);
        var typedLocationId = LocationId.Create(locationId);

        if (await _departmentsRepository.GetByIdAsync(typedDepartmentId, cancellationToken) is null)
        {
            _logger.LogWarning("Department {DepartmentId} does not exist.", departmentId);
            throw new DepartmentNotFoundException(departmentId);
        }

        if (await _locationsRepository.GetByIdAsync(typedLocationId, cancellationToken) is null)
        {
            _logger.LogWarning("Location {LocationId} does not exist.", locationId);
            throw new LocationNotFoundException(locationId);
        }

        return (typedDepartmentId, typedLocationId);
    }
}
