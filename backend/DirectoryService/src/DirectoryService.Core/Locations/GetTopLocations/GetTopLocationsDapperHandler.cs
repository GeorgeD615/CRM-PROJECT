using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DirectoryService.Core.Locations.GetTopLocations;

/// <summary>
/// Тот же топ локаций, но на Dapper: один SQL с LEFT JOIN, группировкой по локации,
/// подсчётом связей и LIMIT. LEFT JOIN оставляет локации без подразделений в выдаче
/// с нулём, DTO материализуется напрямую из строк результата.
/// </summary>
public sealed class GetTopLocationsDapperHandler(
    IDbConnectionFactory connectionFactory,
    ILogger<GetTopLocationsDapperHandler> logger)
    : IQueryHandler<IReadOnlyCollection<GetTopLocationDto>, GetTopLocationsQuery>
{
    private const string TopLocationsSql = """
        SELECT
            l.id                      AS "Id",
            l.name                    AS "Name",
            l.address ->> 'city'      AS "City",
            l.address ->> 'street'    AS "Street",
            l.address ->> 'house'     AS "House",
            l.address ->> 'apartment' AS "Apartment",
            COUNT(dl.id)::int         AS "DepartmentCount"
        FROM locations AS l
        LEFT JOIN department_locations AS dl ON dl.location_id = l.id
        GROUP BY l.id
        ORDER BY COUNT(dl.id) DESC, l.name
        LIMIT @Limit
        """;

    public async Task<Result<IReadOnlyCollection<GetTopLocationDto>, Failure>> HandleAsync(
        GetTopLocationsQuery query,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = connectionFactory.Create();

        var command = new CommandDefinition(
            TopLocationsSql,
            new { Limit = GetTopLocationsQuery.TopSize },
            cancellationToken: cancellationToken);

        IReadOnlyCollection<GetTopLocationDto> result =
            (await connection.QueryAsync<GetTopLocationDto>(command)).AsList();

        logger.LogInformation("Top locations by department count returned {Count} rows.", result.Count);

        return Result.Success<IReadOnlyCollection<GetTopLocationDto>, Failure>(result);
    }
}
