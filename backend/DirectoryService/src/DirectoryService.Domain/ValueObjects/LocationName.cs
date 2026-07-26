using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record LocationName
{
    public const int MaxLength = 150;

    private LocationName(string value) => Value = value;

    public string Value { get; }

    public static Result<LocationName, Failure> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Failure.FromError(Error.Validation("Имя локации обязательно.", code: "location.name.required"));

        string normalized = value.Trim();

        if (normalized.Length > MaxLength)
            return Failure.FromError(Error.Validation(
                $"Имя локации не должно превышать {MaxLength} символов.", code: "location.name.too_long"));

        return new LocationName(normalized);
    }
}
