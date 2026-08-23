using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.GetLocationById;

public record GetLocationByIdQuery(Guid LocationId) : IQuery;
