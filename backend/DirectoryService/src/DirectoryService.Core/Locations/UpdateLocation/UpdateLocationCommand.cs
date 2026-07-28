using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.UpdateLocation;

public record UpdateLocationCommand(Guid LocationId, UpdateLocationRequest UpdateLocationDto) : ICommand;
