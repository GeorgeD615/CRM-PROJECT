namespace DirectoryService.Web.EndpointResults;

/// <summary>
/// Успешный ответ с данными: пишет <see cref="Envelope{TValue}"/> с указанным статусом (200 или 201).
/// </summary>
public sealed class SuccessResult<TValue>(TValue value, int statusCode = StatusCodes.Status200OK) : IResult
{
    private readonly TValue _value = value;
    private readonly int _statusCode = statusCode;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = _statusCode;

        return httpContext.Response.WriteAsJsonAsync(Envelope<TValue>.Ok(_value), cancellationToken: httpContext.RequestAborted);
    }
}

/// <summary>
/// Успешный ответ без данных: пишет пустой <see cref="Envelope"/> с указанным статусом.
/// </summary>
public sealed class SuccessResult(int statusCode = StatusCodes.Status200OK) : IResult
{
    private readonly int _statusCode = statusCode;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = _statusCode;

        return httpContext.Response.WriteAsJsonAsync(Envelope.Ok(), cancellationToken: httpContext.RequestAborted);
    }
}
