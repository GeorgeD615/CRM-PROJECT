namespace DirectoryService.Core.Departments;

/// <summary>
/// Нарушение бизнес-правила: часть указанных локаций не существует.
/// Подразделение не создаётся, пока все локации из запроса не найдены.
/// </summary>
public sealed class LocationsNotFoundException : Exception
{
    public LocationsNotFoundException(IReadOnlyCollection<Guid> locationIds)
        : base($"Locations do not exist: {string.Join(", ", locationIds)}.")
    {
        LocationIds = locationIds;
    }

    public IReadOnlyCollection<Guid> LocationIds { get; }
}
