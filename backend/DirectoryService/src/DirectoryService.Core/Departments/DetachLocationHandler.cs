using DirectoryService.Core.Database;
using DirectoryService.Core.Departments.Exceptions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий отвязки локации от подразделения: удаляет существующую связь.
/// </summary>
public sealed class DetachLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DetachLocationHandler> logger)
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<DetachLocationHandler> _logger = logger;

    public async Task HandleAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        DepartmentLocation? link = await _departmentsRepository.GetDepartmentLocationAsync(
            DepartmentId.Create(departmentId),
            LocationId.Create(locationId),
            cancellationToken);

        if (link is null)
            throw new DepartmentLocationNotFoundException(departmentId, locationId);

        _departmentsRepository.RemoveDepartmentLocation(link);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Location {LocationId} detached from department {DepartmentId}.",
            locationId,
            departmentId);
    }
}
