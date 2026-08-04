using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest CreateLocationDto) : IValidatedCommand;
