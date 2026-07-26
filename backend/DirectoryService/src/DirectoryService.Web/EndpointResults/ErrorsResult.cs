using DirectoryService.Shared;

namespace DirectoryService.Web.EndpointResults;

/// <summary>
/// Единственное место маппинга <see cref="Failure"/> → HTTP-статус и <see cref="Envelope"/>.
/// Один тип ошибок → соответствующий статус; разнотипные ошибки → 500.
/// </summary>
public sealed class ErrorsResult : IResult
{
    private readonly Failure _errors;

    public ErrorsResult(Error error) => _errors = Failure.FromError(error);

    public ErrorsResult(Failure errors) => _errors = errors;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        httpContext.Response.StatusCode = ResolveStatusCode(_errors);

        return httpContext.Response.WriteAsJsonAsync(Envelope.Error(_errors), cancellationToken: httpContext.RequestAborted);
    }

    private static int ResolveStatusCode(Failure errors)
    {
        ErrorType[] distinctTypes = errors.Select(error => error.Type).Distinct().ToArray();

        if (distinctTypes.Length != 1)
            return StatusCodes.Status500InternalServerError;

        return distinctTypes[0] switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Internal => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
