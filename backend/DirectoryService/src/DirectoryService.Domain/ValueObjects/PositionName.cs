using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record PositionName
{
    public const int MaxLength = 150;

    private PositionName(string value) => Value = value;

    public string Value { get; }

    public static Result<PositionName, Failure> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Failure.FromError(Error.Validation("Название должности обязательно.", code: "position.name.required"));

        string normalized = value.Trim();

        if (normalized.Length > MaxLength)
            return Failure.FromError(Error.Validation(
                $"Название должности не должно превышать {MaxLength} символов.", code: "position.name.too_long"));

        return new PositionName(normalized);
    }
}
