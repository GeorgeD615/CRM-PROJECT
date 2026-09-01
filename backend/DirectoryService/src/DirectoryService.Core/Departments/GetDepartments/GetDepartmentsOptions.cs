namespace DirectoryService.Core.Departments.GetDepartments;

/// <summary>
/// Значения по умолчанию и белые списки для списка подразделений. Валидатор проверяет
/// присланные клиентом sortBy/sortDir по этим наборам, а handler выбирает выражение
/// сортировки switch-ем — произвольная строка до LINQ не доходит.
/// </summary>
public static class GetDepartmentsOptions
{
    public const int DefaultPage = 1;

    public const int DefaultPageSize = 20;

    public const int MaxPageSize = 100;

    public const int MaxSearchLength = 100;

    public const string SortByName = "name";

    public const string SortByCreatedAt = "createdAt";

    public const string SortAscending = "asc";

    public const string SortDescending = "desc";

    public static IReadOnlyList<string> SortFields { get; } = [SortByName, SortByCreatedAt];

    public static IReadOnlyList<string> SortDirections { get; } = [SortAscending, SortDescending];

    public static bool IsKnownSortField(string sortBy) =>
        SortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);

    public static bool IsKnownSortDirection(string sortDir) =>
        SortDirections.Contains(sortDir, StringComparer.OrdinalIgnoreCase);
}
