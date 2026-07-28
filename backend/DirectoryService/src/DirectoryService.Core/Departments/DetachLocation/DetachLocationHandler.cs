using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.DetachLocation;

/// <summary>
/// Сценарий отвязки локации от подразделения: удаляет существующую связь.
/// Не бросает; успех логируется как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class DetachLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DetachLocationHandler> logger) : ICommandHandler<DetachLocationCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<DetachLocationHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(DetachLocationCommand command, CancellationToken cancellationToken)
    {
        Guid departmentId = command.DepartmentId;
        Guid locationId = command.LocationId;

        Result<DepartmentLocation, Failure> linkResult = await _departmentsRepository.GetDepartmentLocationAsync(
            DepartmentId.Create(departmentId),
            LocationId.Create(locationId),
            cancellationToken);
        if (linkResult.IsFailure)
            return linkResult.Error;

        UnitResult<Failure> removeResult = _departmentsRepository.RemoveDepartmentLocation(linkResult.Value);
        if (removeResult.IsFailure)
            return removeResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Location {LocationId} detached from department {DepartmentId}.",
            locationId,
            departmentId);

        return UnitResult.Success<Failure>();
    }
}
