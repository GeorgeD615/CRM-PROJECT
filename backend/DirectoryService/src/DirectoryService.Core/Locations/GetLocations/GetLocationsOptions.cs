namespace DirectoryService.Core.Locations.GetLocations;

/// <summary>
/// Значения по умолчанию и белые списки для списка локаций. Валидатор проверяет присланные
/// клиентом sortBy/sortDir по этим наборам, а handler переводит их в заранее известные
/// SQL-выражения — сырое значение в ORDER BY не попадает.
/// </summary>
public static class GetLocationsOptions
{
    public const int DefaultPage = 1;

    public const int DefaultPageSize = 20;

    public const int MaxPageSize = 100;

    public const int MaxSearchLength = 100;

    public const string SortByName = "name";

    public const string SortByCreatedAt = "createdAt";

    public const string SortByDepartmentCount = "departmentCount";

    public const string SortAscending = "asc";

    public const string SortDescending = "desc";

    public static IReadOnlyList<string> SortFields { get; } = [SortByName, SortByCreatedAt, SortByDepartmentCount];

    public static IReadOnlyList<string> SortDirections { get; } = [SortAscending, SortDescending];

    public static bool IsKnownSortField(string sortBy) =>
        SortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);

    public static bool IsKnownSortDirection(string sortDir) =>
        SortDirections.Contains(sortDir, StringComparer.OrdinalIgnoreCase);
}
