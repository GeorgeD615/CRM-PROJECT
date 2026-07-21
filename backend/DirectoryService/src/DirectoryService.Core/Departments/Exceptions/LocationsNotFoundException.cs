using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: часть указанных локаций не существует.
/// Подразделение не создаётся, пока все локации из запроса не найдены.
/// </summary>
public sealed class LocationsNotFoundException(IReadOnlyCollection<Guid> locationIds) :
    NotFoundException([.. locationIds.Select(locationId => Error.NotFound($"Локация '{locationId}' не найдена."))]);
