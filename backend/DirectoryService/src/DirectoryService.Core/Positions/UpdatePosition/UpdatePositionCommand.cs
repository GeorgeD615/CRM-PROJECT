using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Positions.UpdatePosition;

public record UpdatePositionCommand(Guid PositionId, UpdatePositionRequest UpdatePositionDto) : IValidatedCommand;
