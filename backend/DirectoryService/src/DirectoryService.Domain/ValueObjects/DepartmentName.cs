using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentName
{
    public const int MaxLength = 150;

    private DepartmentName(string value) => Value = value;

    public string Value { get; }

    public static Result<DepartmentName, Failure> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Failure.FromError(Error.Validation("Имя подразделения обязательно.", code: "department.name.required"));

        string normalized = value.Trim();

        if (normalized.Length > MaxLength)
            return Failure.FromError(Error.Validation(
                $"Имя подразделения не должно превышать {MaxLength} символов.", code: "department.name.too_long"));

        return new DepartmentName(normalized);
    }
}
