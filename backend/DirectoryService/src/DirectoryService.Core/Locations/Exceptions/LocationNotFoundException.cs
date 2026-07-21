using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Locations.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: локация с указанным id не существует.
/// </summary>
public sealed class LocationNotFoundException(Guid locationId) :
    NotFoundException([Error.NotFound($"Локация '{locationId}' не найдена.")]);
