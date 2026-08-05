using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Positions.DeletePosition;

public record DeletePositionCommand(Guid PositionId) : ICommand;
