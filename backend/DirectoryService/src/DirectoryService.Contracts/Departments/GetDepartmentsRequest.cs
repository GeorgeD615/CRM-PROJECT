namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Параметры списка подразделений из query string. Все поля необязательны: не переданное
/// значение означает поведение по умолчанию (первая страница по 20 записей, сортировка по имени
/// по возрастанию, без фильтра), а переданное — проверяется валидатором.
/// </summary>
public sealed record GetDepartmentsRequest(
    string? Search = null,
    string? SortBy = null,
    string? SortDir = null,
    int? Page = null,
    int? PageSize = null);
