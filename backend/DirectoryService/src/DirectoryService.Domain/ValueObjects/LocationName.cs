namespace DirectoryService.Domain.ValueObjects;

public sealed record LocationName
{
    public const int MaxLength = 150;

    private LocationName(string value) => Value = value;

    public string Value { get; }

    public static LocationName Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length == 0)
            throw new ArgumentException("Location name must not be empty.", nameof(value));

        if (normalized.Length > MaxLength)
            throw new ArgumentException(
                $"Location name must not exceed {MaxLength} characters.", nameof(value));

        return new LocationName(normalized);
    }
}
