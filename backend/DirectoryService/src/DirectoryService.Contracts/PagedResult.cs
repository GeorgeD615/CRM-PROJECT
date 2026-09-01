namespace DirectoryService.Contracts;

/// <summary>
/// Страница списка для UI: элементы текущей страницы плюс <see cref="TotalCount"/> —
/// общее число строк под тем же фильтром, без которого пагинатор не сможет посчитать страницы.
/// </summary>
public sealed class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }
}
