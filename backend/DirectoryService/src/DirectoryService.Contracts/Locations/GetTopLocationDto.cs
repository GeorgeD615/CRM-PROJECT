namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Строка аналитического блока «топ локаций»: плоский набор полей без доменных типов
/// и без EF-навигаций. <see cref="DepartmentCount"/> — число привязанных подразделений.
/// </summary>
public sealed class GetTopLocationDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required string Street { get; init; }

    public required string House { get; init; }

    public required string Apartment { get; init; }

    public int DepartmentCount { get; init; }
}
