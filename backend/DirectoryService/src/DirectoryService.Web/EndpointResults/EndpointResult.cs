using CSharpFunctionalExtensions;
using DirectoryService.Shared;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Web.EndpointResults;

/// <summary>
/// Граница контроллера для сценариев с данными. Неявно превращает <see cref="Result{TValue, Failure}"/>
/// в success (200) либо в <see cref="ErrorsResult"/>. Для создания ресурса используйте <see cref="Created"/> (201).
/// </summary>
public sealed class EndpointResult<TValue> : IResult
{
    private readonly IResult _result;

    private EndpointResult(IResult result) => _result = result;

    public EndpointResult(Result<TValue, Failure> result)
        : this(result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult(result.Error))
    {
    }

    public Task ExecuteAsync(HttpContext httpContext) => _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Failure> result) => new(result);

    public static EndpointResult<TValue> Created(Result<TValue, Failure> result) =>
        new(result.IsSuccess
            ? new SuccessResult<TValue>(result.Value, StatusCodes.Status201Created)
            : new ErrorsResult(result.Error));
}

/// <summary>
/// Граница контроллера для сценариев без данных. Неявно превращает <see cref="UnitResult{Failure}"/>
/// в success (200) либо в <see cref="ErrorsResult"/>. Для создания ресурса используйте <see cref="Created"/> (201).
/// </summary>
public sealed class EndpointResult : IResult
{
    private readonly IResult _result;

    private EndpointResult(IResult result) => _result = result;

    public EndpointResult(UnitResult<Failure> result)
        : this(result.IsSuccess
            ? new SuccessResult()
            : new ErrorsResult(result.Error))
    {
    }

    public Task ExecuteAsync(HttpContext httpContext) => _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult(UnitResult<Failure> result) => new(result);

    public static EndpointResult Created(UnitResult<Failure> result) =>
        new(result.IsSuccess
            ? new SuccessResult(StatusCodes.Status201Created)
            : new ErrorsResult(result.Error));
}
