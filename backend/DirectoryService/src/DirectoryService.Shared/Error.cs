using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DirectoryService.Shared;

/// <summary>
/// Типизированная ошибка
/// </summary>
public sealed class Error
{
    private const string SEPARATOR = "||";

    [JsonConstructor]
    private Error(string code, string message, ErrorType type, string? invalidProperty)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidProperty = invalidProperty;
    }

    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }

    public string? InvalidProperty { get; }

    public static Error None => new(string.Empty, string.Empty, ErrorType.None, null);

    public static Error Validation(string message, string? invalidProperty = null, string? code = null) =>
        new(code ?? "value.is.invalid", message, ErrorType.Validation, invalidProperty);

    public static Error NotFound(string message, string? invalidProperty = null, string? code = null) =>
        new(code ?? "record.not.found", message, ErrorType.NotFound, invalidProperty);

    public static Error Conflict(string message, string? invalidProperty = null, string? code = null) =>
        new(code ?? "value.is.conflict", message, ErrorType.Conflict, invalidProperty);

    public static Error Internal(string message, string? invalidProperty = null, string? code = null) =>
        new(code ?? "server.internal.error", message, ErrorType.Internal, invalidProperty);

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type, InvalidProperty ?? string.Empty);
    }

    public static Error Deserialize(string serialized)
    {
        if (!TryDeserialize(serialized, out Error? error))
        {
            throw new ArgumentException("Invalid serialized format");
        }

        return error;
    }

    public static bool TryDeserialize(string serialized, [NotNullWhen(true)] out Error? error)
    {
        error = null;

        if (string.IsNullOrEmpty(serialized))
            return false;

        string[] parts = serialized.Split(SEPARATOR);

        if (parts.Length < 4)
            return false;

        if (!Enum.TryParse<ErrorType>(parts[2], out var type))
            return false;

        string? invProp = string.IsNullOrEmpty(parts[3]) ? null : parts[3];

        error = new Error(parts[0], parts[1], type, invProp);
        return true;
    }
}
