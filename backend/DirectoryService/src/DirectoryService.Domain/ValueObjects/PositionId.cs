namespace DirectoryService.Domain.ValueObjects;

public sealed record PositionId
{
    private PositionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static PositionId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Position id must not be empty.", nameof(value));

        return new PositionId(value);
    }
}
