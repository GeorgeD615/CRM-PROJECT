using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Positions.CreatePosition;

/// <summary>
/// Сценарий создания должности: проверяет уникальность имени и возвращает id созданной должности.
/// Не бросает; успех логируется как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class CreatePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<CreatePositionHandler> logger) : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IPositionsRepository _positionsRepository = positionsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<CreatePositionHandler> _logger = logger;

    public async Task<Result<Guid, Failure>> HandleAsync(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        CreatePositionRequest request = command.CreatePositionDto;

        PositionName name = PositionName.Create(request.Name).Value;

        bool isNameTaken = await _positionsRepository.IsNameTakenAsync(name, cancellationToken);
        if (isNameTaken)
        {
            _logger.LogWarning("Position name {PositionName} is already taken.", name.Value);

            return Failure.From(Error.Conflict($"Должность с именем '{name.Value}' уже существует.", code: "directory.position.name_conflict"));
        }

        Result<Position, Failure> positionResult = Position.Create(name);
        if (positionResult.IsFailure)
            return positionResult.Error;

        Position position = positionResult.Value;

        _positionsRepository.Add(position);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Position {PositionId} created with name {PositionName}.", position.Id.Value, position.Name.Value);

        return position.Id.Value;
    }
}
