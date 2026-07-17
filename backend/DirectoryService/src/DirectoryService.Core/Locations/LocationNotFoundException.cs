namespace DirectoryService.Core.Locations;

/// <summary>
/// Нарушение бизнес-правила: локация с указанным id не существует.
/// </summary>
public sealed class LocationNotFoundException : Exception
{
    public LocationNotFoundException(Guid locationId)
        : base($"Локация '{locationId}' не найдена.")
    {
        LocationId = locationId;
    }

    public Guid LocationId { get; }
}
