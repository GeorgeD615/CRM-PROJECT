using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed partial record DepartmentSlug
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    private DepartmentSlug(string value) => Value = value;

    public string Value { get; }

    public static Result<DepartmentSlug, Failure> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Failure.FromError(Error.Validation("Slug подразделения обязателен.", code: "department.slug.required"));

        string normalized = value.Trim().ToLowerInvariant();

        var errors = new List<Error>();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            errors.Add(Error.Validation(
                $"Slug должен быть от {MinLength} до {MaxLength} символов.", code: "department.slug.length"));

        if (!SlugPattern().IsMatch(normalized))
            errors.Add(Error.Validation(
                "Slug может содержать только строчные латинские буквы, цифры и дефис, " +
                "и не может начинаться или заканчиваться дефисом.",
                code: "department.slug.format"));

        if (errors.Count > 0)
            return new Failure(errors);

        return new DepartmentSlug(normalized);
    }

    [GeneratedRegex(@"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex SlugPattern();
}
