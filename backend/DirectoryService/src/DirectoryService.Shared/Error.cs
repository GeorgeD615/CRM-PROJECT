using System.Text.Json.Serialization;

namespace DirectoryService.Shared;

/// <summary>
/// Типизированная ошибка
/// </summary>
public sealed class Error
{
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
}
