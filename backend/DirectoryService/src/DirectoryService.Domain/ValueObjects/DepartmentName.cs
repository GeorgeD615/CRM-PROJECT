namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentName
{
    public const int MaxLength = 150;

    private DepartmentName(string value) => Value = value;

    public string Value { get; }

    public static DepartmentName Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length == 0)
            throw new ArgumentException("Department name must not be empty.", nameof(value));

        if (normalized.Length > MaxLength)
            throw new ArgumentException(
                $"Department name must not exceed {MaxLength} characters.", nameof(value));

        return new DepartmentName(normalized);
    }
}
