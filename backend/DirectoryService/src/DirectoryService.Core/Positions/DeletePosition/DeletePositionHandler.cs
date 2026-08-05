using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.DeletePosition;

/// <summary>
/// Сценарий удаления должности: запрещает удаление, пока должность привязана к подразделениям
/// (её нельзя удалить, пока на неё ссылаются). Не бросает; commit — через TransactionManager.
/// </summary>
public sealed class DeletePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<DeletePositionHandler> logger) : ICommandHandler<DeletePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository = positionsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<DeletePositionHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(DeletePositionCommand command, CancellationToken cancellationToken)
    {
        var positionId = PositionId.Create(command.PositionId);

        Result<Position, Failure> positionResult = await _positionsRepository.GetByIdAsync(positionId, cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        bool hasLinks = await _positionsRepository.HasLinkedDepartmentsAsync(positionId, cancellationToken);
        if (hasLinks)
        {
            _logger.LogWarning("Position {PositionId} cannot be deleted while attached to departments.", command.PositionId);

            return Failure.From(Error.Conflict(
                "Нельзя удалить должность, пока она привязана к подразделениям.",
                code: "directory.position.has_links"));
        }

        _positionsRepository.Remove(positionResult.Value);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Position {PositionId} deleted.", command.PositionId);

        return UnitResult.Success<Failure>();
    }
}
