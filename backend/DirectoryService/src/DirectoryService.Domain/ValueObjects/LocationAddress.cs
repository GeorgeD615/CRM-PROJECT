namespace DirectoryService.Domain.ValueObjects;

public sealed record LocationAddress
{
    public const int MaxLength = 500;

    private LocationAddress(string value) => Value = value;

    public string Value { get; }

    public static LocationAddress Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length == 0)
            throw new ArgumentException("Location address must not be empty.", nameof(value));

        if (normalized.Length > MaxLength)
            throw new ArgumentException(
                $"Location address must not exceed {MaxLength} characters.", nameof(value));

        return new LocationAddress(normalized);
    }
}
