namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Параметры списка локаций из query string. Все поля необязательны: не переданное значение
/// означает поведение по умолчанию (без фильтров, сортировка по имени по возрастанию,
/// первая страница по 20 записей), а переданное — проверяется валидатором.
/// </summary>
public sealed record GetLocationsRequest(
    string? Search = null,
    int? MinDepartmentCount = null,
    string? SortBy = null,
    string? SortDir = null,
    int? Page = null,
    int? PageSize = null);
