using DirectoryService.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Extensions;

/// <summary>
/// Маппинг <see cref="Failure"/> в HTTP-ответ
/// </summary>
public static class ResponseExtensions
{
    public static IActionResult ToResponse(this Failure failure)
    {
        ArgumentNullException.ThrowIfNull(failure);

        int statusCode = ResolveStatusCode(failure);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
        };
        problemDetails.Extensions["errors"] = failure.ToArray();

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
        };
    }

    private static int ResolveStatusCode(Failure failure)
    {
        ErrorType[] distinctTypes = failure.Select(error => error.Type).Distinct().ToArray();

        if (distinctTypes.Length != 1)
        {
            return StatusCodes.Status500InternalServerError;
        }

        return distinctTypes[0] switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Internal => StatusCodes.Status500InternalServerError,
            ErrorType.None => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
