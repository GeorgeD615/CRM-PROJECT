using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.DeleteLocation;

public record DeleteLocationCommand(Guid LocationId) : ICommand;
