using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.GetTopLocations;

/// <summary>
/// Топ локаций по числу подразделений на EF Core. Счётчик считается одной группировкой
/// по department_locations, которая приджойнивается к локациям слева: локации без
/// подразделений остаются в выдаче с нулём. Всё уходит в БД одним запросом
/// (JOIN + GROUP BY + ORDER BY + LIMIT), DTO собирается прямо в проекции.
/// </summary>
public sealed class GetTopLocationsEfCoreHandler(
    IReadDbContext readDbContext,
    ILogger<GetTopLocationsEfCoreHandler> logger)
    : IQueryHandler<IReadOnlyCollection<GetTopLocationDto>, GetTopLocationsQuery>
{
    public async Task<Result<IReadOnlyCollection<GetTopLocationDto>, Failure>> HandleAsync(
        GetTopLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var departmentCounts = readDbContext.DepartmentLocations
            .GroupBy(departmentLocation => departmentLocation.LocationId)
            .Select(group => new { LocationId = group.Key, DepartmentCount = (int?)group.Count() });

        IQueryable<GetTopLocationDto> topLocations =
            from location in readDbContext.Locations
            join counts in departmentCounts on location.Id equals counts.LocationId into locationCounts
            from counts in locationCounts.DefaultIfEmpty()
            orderby counts.DepartmentCount ?? 0 descending, location.Name
            select new GetTopLocationDto
            {
                Id = location.Id.Value,
                Name = location.Name.Value,
                City = location.Address.City,
                Street = location.Address.Street,
                House = location.Address.House,
                Apartment = location.Address.Apartment,
                DepartmentCount = counts.DepartmentCount ?? 0,
            };

        IReadOnlyCollection<GetTopLocationDto> result = await topLocations
            .Take(GetTopLocationsQuery.TopSize)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Top locations by department count returned {Count} rows.", result.Count);

        return Result.Success<IReadOnlyCollection<GetTopLocationDto>, Failure>(result);
    }
}
