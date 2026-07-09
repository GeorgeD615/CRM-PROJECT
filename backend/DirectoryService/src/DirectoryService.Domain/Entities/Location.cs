using DirectoryService.Domain.ValueObjects;

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

    public LocationId Id { get; }

    public LocationName Name { get; private set; }

    public LocationAddress Address { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public static Location Create(LocationName name, LocationAddress address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);

        return new Location(LocationId.Create(Guid.CreateVersion7()), name, address);
    }

    public void Rename(LocationName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeAddress(LocationAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
}
