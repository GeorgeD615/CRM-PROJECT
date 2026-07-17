using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий отвязки локации от подразделения: удаляет существующую связь.
/// </summary>
public sealed class DetachLocationHandler
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DetachLocationHandler> _logger;

    public DetachLocationHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        ILogger<DetachLocationHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task HandleAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        DepartmentLocation? link = await _departmentsRepository.GetDepartmentLocationAsync(
            DepartmentId.Create(departmentId),
            LocationId.Create(locationId),
            cancellationToken);

        if (link is null)
        {
            _logger.LogWarning(
                "Link between department {DepartmentId} and location {LocationId} does not exist.",
                departmentId,
                locationId);

            throw new DepartmentLocationNotFoundException(departmentId, locationId);
        }

        _departmentsRepository.RemoveDepartmentLocation(link);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Location {LocationId} detached from department {DepartmentId}.",
            locationId,
            departmentId);
    }
}
