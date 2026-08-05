using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.UpdatePosition;

/// <summary>
/// Сценарий переименования должности: не допускает дубля имени с другой должностью.
/// Не бросает; успех логируется как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class UpdatePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdatePositionHandler> logger) : ICommandHandler<UpdatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository = positionsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<UpdatePositionHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        Guid positionId = command.PositionId;
        UpdatePositionRequest request = command.UpdatePositionDto;

        Result<Position, Failure> positionResult = await _positionsRepository.GetByIdAsync(
            PositionId.Create(positionId),
            cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        Position position = positionResult.Value;

        PositionName name = PositionName.Create(request.Name).Value;

        if (name != position.Name)
        {
            bool isNameTaken = await _positionsRepository.IsNameTakenAsync(name, cancellationToken);
            if (isNameTaken)
            {
                _logger.LogWarning("Position name {PositionName} is already taken.", name.Value);

                return Failure.From(Error.Conflict($"Должность с именем '{name.Value}' уже существует.", code: "directory.position.name_conflict"));
            }
        }

        UnitResult<Failure> renameResult = position.Rename(name);
        if (renameResult.IsFailure)
            return renameResult.Error;

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Position {PositionId} renamed to {PositionName}.", positionId, name.Value);

        return UnitResult.Success<Failure>();
    }
}
