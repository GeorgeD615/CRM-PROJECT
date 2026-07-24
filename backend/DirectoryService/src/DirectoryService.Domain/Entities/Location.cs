using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Domain.Entities;

/// <summary>
/// Локация — физическое место, где работают подразделения (офис, площадка).
/// Связывается с подразделениями через <see cref="DepartmentLocation"/>.
/// </summary>
public sealed class Location
{
    private Location(LocationId id, LocationName name, LocationAddress address)
    {
        Id = id;
        Name = name;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    // Для EF Core: owned-навигацию Address нельзя привязать к параметру конструктора,
    // она заполняется материализатором после создания объекта.
    private Location(LocationId id, LocationName name)
    {
        Id = id;
        Name = name;
        Address = null!;
    }

    public LocationId Id { get; }

    public LocationName Name { get; private set; }

    public LocationAddress Address { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public static Result<Location, Failure> Create(LocationName name, LocationAddress address)
    {
        var errors = new List<Error>();

        if (name is null)
            errors.Add(Error.Validation("Имя локации обязательно.", nameof(name)));

        if (address is null)
            errors.Add(Error.Validation("Адрес локации обязателен.", nameof(address)));

        if (errors.Count > 0)
            return new Failure(errors);

        // Проверки выше гарантируют, что оба значения не null, когда список ошибок пуст.
        return new Location(LocationId.Create(Guid.CreateVersion7()), name!, address!);
    }

    public UnitResult<Failure> Rename(LocationName name)
    {
        if (name is null)
            return Failure.From(Error.Validation("Имя локации обязательно.", nameof(name)));

        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public UnitResult<Failure> ChangeAddress(LocationAddress address)
    {
        if (address is null)
            return Failure.From(Error.Validation("Адрес локации обязателен.", nameof(address)));

        Address = address;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }
}
