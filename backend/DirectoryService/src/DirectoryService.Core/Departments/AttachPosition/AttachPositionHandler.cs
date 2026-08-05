using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.AttachPosition;

/// <summary>
/// Сценарий привязки должности к подразделению: проверяет, что обе стороны существуют и связи ещё нет,
/// и создаёт связь. Повтор пары даёт conflict и не создаёт дубль. Не бросает; commit — через TransactionManager.
/// </summary>
public sealed class AttachPositionHandler(
    IDepartmentsRepository departmentsRepository,
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<AttachPositionHandler> logger) : ICommandHandler<AttachPositionCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly IPositionsRepository _positionsRepository = positionsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<AttachPositionHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(AttachPositionCommand command, CancellationToken cancellationToken)
    {
        Guid departmentId = command.DepartmentId;
        Guid positionId = command.PositionId;

        var typedDepartmentId = DepartmentId.Create(departmentId);
        var typedPositionId = PositionId.Create(positionId);

        Result<Department, Failure> departmentResult =
            await _departmentsRepository.GetByIdAsync(typedDepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        Result<Position, Failure> positionResult =
            await _positionsRepository.GetByIdAsync(typedPositionId, cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        Result<DepartmentPosition, Failure> existingLinkResult = await _departmentsRepository.GetDepartmentPositionAsync(
            typedDepartmentId,
            typedPositionId,
            cancellationToken);

        if (existingLinkResult.IsSuccess)
        {
            _logger.LogWarning(
                "Position {PositionId} is already attached to department {DepartmentId}.",
                positionId,
                departmentId);

            return Failure.From(Error.Conflict(
                $"Должность '{positionId}' уже привязана к подразделению '{departmentId}'.",
                code: "directory.department_position.conflict"));
        }

        // existingLinkResult.IsFailure здесь означает «связь не найдена» — ожидаемый путь.
        var link = DepartmentPosition.Create(typedDepartmentId, typedPositionId);

        _departmentsRepository.AddDepartmentPosition(link);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation(
            "Position {PositionId} attached to department {DepartmentId}.",
            positionId,
            departmentId);

        return UnitResult.Success<Failure>();
    }
}
