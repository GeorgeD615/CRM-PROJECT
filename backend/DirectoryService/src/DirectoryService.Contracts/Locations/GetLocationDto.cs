namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Карточка локации для UI: плоский набор полей без доменных типов
/// и без коллекций связанных сущностей.
/// </summary>
public sealed class GetLocationDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required string Street { get; init; }

    public required string House { get; init; }

    public required string Apartment { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
