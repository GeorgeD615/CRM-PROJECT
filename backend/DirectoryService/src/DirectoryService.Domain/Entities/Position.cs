using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

/// <summary>
/// Должность — роль, доступная внутри подразделений.
/// Связывается с подразделениями через <see cref="DepartmentPosition"/>.
/// </summary>
public sealed class Position
{
    private Position(PositionId id, PositionName name)
    {
        Id = id;
        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public PositionId Id { get; }

    public PositionName Name { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public static Position Create(PositionName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return new Position(PositionId.Create(Guid.CreateVersion7()), name);
    }

    public void Rename(PositionName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}
