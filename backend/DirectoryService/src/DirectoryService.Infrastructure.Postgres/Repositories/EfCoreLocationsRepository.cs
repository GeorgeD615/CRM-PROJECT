using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище локаций на EF Core: работает через <see cref="AppDbContext"/>.
/// Единственное место, где ошибки обращения к БД логируются и превращаются
/// в <see cref="ErrorType.Internal"/> без утечки деталей исключения наружу.
/// </summary>
public sealed class EfCoreLocationsRepository(
    AppDbContext dbContext,
    ILogger<EfCoreLocationsRepository> logger) : ILocationsRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<EfCoreLocationsRepository> _logger = logger;

    public async Task<Result<Location, Failure>> GetByIdAsync(LocationId id, CancellationToken cancellationToken)
    {
        try
        {
            Location? location = await _dbContext.Locations
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (location is null)
                return Failure.From(Error.NotFound($"Локация '{id.Value}' не найдена.", code: "directory.location.not_found"));

            return location;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load location {LocationId}.", id.Value);
            return Failure.From(Error.Internal("Не удалось получить локацию."));
        }
    }

    public async Task<Result<bool, Failure>> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken)
    {
        try
        {
            bool isTaken = await _dbContext.Locations.AnyAsync(l => l.Name == name, cancellationToken);

            return isTaken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check whether location name {LocationName} is taken.", name.Value);
            return Failure.From(Error.Internal("Не удалось проверить имя локации."));
        }
    }

    public async Task<Result<IReadOnlyCollection<LocationId>, Failure>> GetExistingIdsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyCollection<LocationId> existingIds = await _dbContext.Locations
                .Where(l => locationIds.Contains(l.Id))
                .Select(l => l.Id)
                .ToArrayAsync(cancellationToken);

            return Result.Success<IReadOnlyCollection<LocationId>, Failure>(existingIds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load existing location ids.");
            return Failure.From(Error.Internal("Не удалось получить локации."));
        }
    }

    public UnitResult<Failure> Add(Location location)
    {
        try
        {
            _dbContext.Locations.Add(location);

            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add location {LocationId}.", location.Id.Value);
            return Failure.From(Error.Internal("Не удалось добавить локацию."));
        }
    }
}
