using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

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

    public static Result<Position, Failure> Create(PositionName name)
    {
        if (name is null)
            return Failure.From(Error.Validation("Имя должности обязательно.", nameof(name)));

        return new Position(PositionId.Create(Guid.CreateVersion7()), name);
    }

    public UnitResult<Failure> Rename(PositionName name)
    {
        if (name is null)
            return Failure.From(Error.Validation("Имя должности обязательно.", nameof(name)));

        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }
}
