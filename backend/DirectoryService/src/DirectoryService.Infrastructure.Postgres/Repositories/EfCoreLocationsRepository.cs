using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище локаций на EF Core: работает через <see cref="AppDbContext"/>.
/// </summary>
public sealed class EfCoreLocationsRepository : ILocationsRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EfCoreLocationsRepository> _logger;

    public EfCoreLocationsRepository(AppDbContext dbContext, ILogger<EfCoreLocationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<bool> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken) =>
        _dbContext.Locations.AnyAsync(l => l.Name == name, cancellationToken);

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Locations.Add(location);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to save location {LocationId}.", location.Id.Value);

            throw;
        }
    }
}
