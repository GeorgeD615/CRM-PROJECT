using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Web.Middlewares;

/// <summary>
/// «Крыша» pipeline-а: ловит доменные исключения и ошибки валидации, превращает их
/// в предсказуемый JSON-ответ с корректным HTTP-статусом. Статус определяется типом
/// пойманного исключения (<see cref="NotFoundException"/> → 404, <see cref="ConflictException"/> → 409,
/// <see cref="BadRequestException"/>, включая <see cref="ValidationException"/>, → 400).
/// Любое непредвиденное исключение логируется и возвращается как generic 500 —
/// внутренние детали наружу не утекают. Единая точка логирования ошибок запроса.
/// </summary>
public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            LogError(context, exception);

            (int statusCode, Error[] errors) = HandleException(exception);

            await WriteErrorsAsync(context, errors, statusCode);
        }
    }

    private static (int StatusCode, Error[] Errors) HandleException(Exception exception) => exception switch
    {
        NotFoundException => (StatusCodes.Status404NotFound, DeserializeErrors(exception)),
        ConflictException => (StatusCodes.Status409Conflict, DeserializeErrors(exception)),
        BadRequestException => (StatusCodes.Status400BadRequest, DeserializeErrors(exception)),
        _ => (StatusCodes.Status500InternalServerError, [Error.Internal("Произошла непредвиденная ошибка.")]),
    };

    private void LogError(HttpContext context, Exception exception) =>
        _logger.LogError(
            exception,
            "Error while processing {Method} {Path}.",
            context.Request.Method,
            context.Request.Path);

    private static Error[] DeserializeErrors(Exception exception) =>
        JsonSerializer.Deserialize<Error[]>(exception.Message) ?? [];

    private static Task WriteErrorsAsync(HttpContext context, IReadOnlyList<Error> errors, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(errors, SerializerOptions), cancellationToken: context.RequestAborted);
    }
}
