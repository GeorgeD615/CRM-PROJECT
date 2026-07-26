using DirectoryService.Shared;
using DirectoryService.Web.EndpointResults;

namespace DirectoryService.Web.Middlewares;

/// <summary>
/// «Крыша» pipeline-а и страховка от багов: ловит всё, что не прошло через Result, логирует
/// (единая точка логирования непойманных ошибок) и возвращает 500 в том же <see cref="Envelope"/>-формате
/// с безопасным internal-<see cref="Error"/> — без утечки внутренних деталей наружу.
/// </summary>
public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error while processing {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);

            Failure failure = Error.Internal("Произошла непредвиденная ошибка.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(Envelope.Error(failure), cancellationToken: context.RequestAborted);
        }
    }
}
