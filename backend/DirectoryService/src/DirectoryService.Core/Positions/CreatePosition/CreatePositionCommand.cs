using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Positions.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest CreatePositionDto) : IValidatedCommand;
