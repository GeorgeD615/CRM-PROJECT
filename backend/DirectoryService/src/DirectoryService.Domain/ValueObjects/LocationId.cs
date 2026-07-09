namespace DirectoryService.Domain.ValueObjects;

public sealed record LocationId
{
    private LocationId(Guid value) => Value = value;

    public Guid Value { get; }

    public static LocationId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Location id must not be empty.", nameof(value));

        return new LocationId(value);
    }
}
