using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DirectoryService.Core.Locations.GetLocations;

/// <summary>
/// Страница списка локаций с числом привязанных подразделений на Dapper.
/// <para>
/// Фильтрация описана один раз — в CTE <c>filtered_locations</c>, который переиспользуют оба
/// запроса: COUNT по той же выборке и сама страница. Оба уходят в БД одной командой, поэтому
/// totalCount и элементы всегда посчитаны по одинаковым условиям.
/// </para>
/// <para>
/// Все значения от клиента передаются параметрами; из запроса в SQL-текст попадает только
/// выражение ORDER BY, выбранное по белому списку.
/// </para>
/// </summary>
public sealed class GetLocationsHandler(
    IDbConnectionFactory connectionFactory,
    ILogger<GetLocationsHandler> logger)
    : IQueryHandler<PagedResult<LocationListItemDto>, GetLocationsQuery>
{
    private const string FilteredLocationsCte = """
        WITH filtered_locations AS (
            SELECT
                l.id                      AS id,
                l.name                    AS name,
                l.address ->> 'city'      AS city,
                l.address ->> 'street'    AS street,
                l.address ->> 'house'     AS house,
                l.address ->> 'apartment' AS apartment,
                l.created_at              AS created_at,
                COUNT(dl.id)::int         AS department_count
            FROM locations AS l
            LEFT JOIN department_locations AS dl ON dl.location_id = l.id
            WHERE l.name ILIKE @SearchPattern
            GROUP BY l.id
            HAVING COUNT(dl.id) >= @MinDepartmentCount
        )
        """;

    private const string TotalCountSql = $"""
        {FilteredLocationsCte}
        SELECT COUNT(*)::int FROM filtered_locations
        """;

    private const string PageSelectSql = """
        SELECT
            id               AS "Id",
            name             AS "Name",
            city             AS "City",
            street           AS "Street",
            house            AS "House",
            apartment        AS "Apartment",
            created_at       AS "CreatedAt",
            department_count AS "DepartmentCount"
        FROM filtered_locations
        """;

    public async Task<Result<PagedResult<LocationListItemDto>, Failure>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        GetLocationsRequest request = query.GetLocationsDto;

        int page = request.Page ?? GetLocationsOptions.DefaultPage;
        int pageSize = request.PageSize ?? GetLocationsOptions.DefaultPageSize;

        string pageSql = $"""
            {FilteredLocationsCte}
            {PageSelectSql}
            ORDER BY {ResolveOrderBy(request.SortBy, request.SortDir)}
            LIMIT @Limit OFFSET @Offset
            """;

        var parameters = new
        {
            SearchPattern = BuildSearchPattern(request.Search),
            MinDepartmentCount = request.MinDepartmentCount ?? 0,
            Limit = pageSize,
            Offset = (page - 1) * pageSize,
        };

        using IDbConnection connection = connectionFactory.Create();

        // Обе выборки уходят одной командой: totalCount и страница считаются по одному и тому же CTE.
        var command = new CommandDefinition(
            $"{TotalCountSql};\n{pageSql}",
            parameters,
            cancellationToken: cancellationToken);

        using SqlMapper.GridReader reader = await connection.QueryMultipleAsync(command);

        int totalCount = await reader.ReadSingleAsync<int>();
        IReadOnlyCollection<LocationListItemDto> items = (await reader.ReadAsync<LocationListItemDto>()).AsList();

        logger.LogInformation(
            "Locations page {Page} of size {PageSize} returned {Count} of {TotalCount} rows.",
            page,
            pageSize,
            items.Count,
            totalCount);

        return new PagedResult<LocationListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    // Пустой поиск превращается в '%': фильтр остаётся в SQL, но ничего не отсекает.
    // Символы шаблона в пользовательском вводе экранируются, чтобы «50%» искалось буквально.
    private static string BuildSearchPattern(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return "%";

        string escaped = search.Trim()
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);

        return $"%{escaped}%";
    }

    // Dapper параметризует значения, но не имена колонок, поэтому sortBy переводится
    // в одно из заранее известных выражений. Неизвестное значение сюда не доходит —
    // его отсекает валидатор, а этот switch дополнительно возвращает сортировку по умолчанию.
    // id как вторичный ключ: страницы не «плавают» при одинаковых значениях.
    private static string ResolveOrderBy(string? sortBy, string? sortDir)
    {
        string column =
            Matches(sortBy, GetLocationsOptions.SortByCreatedAt) ? "created_at"
            : Matches(sortBy, GetLocationsOptions.SortByDepartmentCount) ? "department_count"
            : "name";

        string direction = Matches(sortDir, GetLocationsOptions.SortDescending) ? "DESC" : "ASC";

        return $"{column} {direction}, id";
    }

    private static bool Matches(string? value, string expected) =>
        string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
}
