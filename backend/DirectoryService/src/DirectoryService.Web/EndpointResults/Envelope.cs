using DirectoryService.Shared;

namespace DirectoryService.Web.EndpointResults;

/// <summary>
/// Единый контракт ответа API. На успехе заполнен <see cref="Result"/>, а <see cref="Errors"/> == null;
/// на ошибке <see cref="Result"/> == null, а <see cref="Errors"/> заполнены. JSON-форма одинакова
/// для всех endpoint-ов Directory Service.
/// </summary>
public sealed record Envelope
{
    private Envelope(object? result, Failure? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public object? Result { get; }

    public Failure? Errors { get; }

    public bool IsError => Errors is not null;

    public DateTime TimeGenerated { get; }

    public static Envelope Ok(object? result = null) => new(result, errors: null);

    public static Envelope Error(Failure errors) => new(result: null, errors);
}

/// <summary>
/// Типизированная версия <see cref="Envelope"/> с полями той же формы — для success-ответов с данными.
/// </summary>
public sealed record Envelope<T>
{
    private Envelope(T? result, Failure? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public T? Result { get; }

    public Failure? Errors { get; }

    public bool IsError => Errors is not null;

    public DateTime TimeGenerated { get; }

    public static Envelope<T> Ok(T? result = default) => new(result, errors: null);

    public static Envelope<T> Error(Failure errors) => new(default, errors);
}
