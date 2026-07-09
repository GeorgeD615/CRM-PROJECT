namespace DirectoryService.Domain.ValueObjects;

public sealed record PositionName
{
    public const int MaxLength = 150;

    private PositionName(string value) => Value = value;

    public string Value { get; }

    public static PositionName Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length == 0)
            throw new ArgumentException("Position name must not be empty.", nameof(value));

        if (normalized.Length > MaxLength)
            throw new ArgumentException(
                $"Position name must not exceed {MaxLength} characters.", nameof(value));

        return new PositionName(normalized);
    }
}
