using System.Text.RegularExpressions;

namespace DirectoryService.Domain.ValueObjects;

public sealed partial record DepartmentSlug
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    private DepartmentSlug(string value) => Value = value;

    public string Value { get; }

    public static DepartmentSlug Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            throw new ArgumentException(
                $"Department slug must be between {MinLength} and {MaxLength} characters.",
                nameof(value));

        if (!SlugPattern().IsMatch(normalized))
            throw new ArgumentException(
                "Department slug must contain only lowercase latin letters, digits and hyphens, " +
                "and must not start or end with a hyphen.",
                nameof(value));

        return new DepartmentSlug(normalized);
    }

    [GeneratedRegex(@"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex SlugPattern();
}
