using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.GetLocations;

public record GetLocationsQuery(GetLocationsRequest GetLocationsDto) : IValidatedQuery;
