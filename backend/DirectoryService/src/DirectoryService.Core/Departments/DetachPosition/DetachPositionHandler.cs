using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.DetachPosition;

/// <summary>
/// Сценарий отвязки должности от подразделения: удаляет существующую связь; отвязка несуществующей
/// пары возвращает not-found. Не бросает; commit — через TransactionManager.
/// </summary>
public sealed class DetachPositionHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DetachPositionHandler> logger) : ICommandHandler<DetachPositionCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<DetachPositionHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(DetachPositionCommand command, CancellationToken cancellationToken)
    {
        Guid departmentId = command.DepartmentId;
        Guid positionId = command.PositionId;

        Result<DepartmentPosition, Failure> linkResult = await _departmentsRepository.GetDepartmentPositionAsync(
            DepartmentId.Create(departmentId),
            PositionId.Create(positionId),
            cancellationToken);
        if (linkResult.IsFailure)
            return linkResult.Error;

        _departmentsRepository.RemoveDepartmentPosition(linkResult.Value);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation(
            "Position {PositionId} detached from department {DepartmentId}.",
            positionId,
            departmentId);

        return UnitResult.Success<Failure>();
    }
}
