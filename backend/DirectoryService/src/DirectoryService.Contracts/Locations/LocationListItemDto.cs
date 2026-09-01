namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Строка таблицы локаций: данные самой локации плюс <see cref="DepartmentCount"/> —
/// число привязанных подразделений. Плоский набор полей, без доменных типов и связанных коллекций.
/// </summary>
public sealed class LocationListItemDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required string Street { get; init; }

    public required string House { get; init; }

    public required string Apartment { get; init; }

    public DateTime CreatedAt { get; init; }

    public int DepartmentCount { get; init; }
}
