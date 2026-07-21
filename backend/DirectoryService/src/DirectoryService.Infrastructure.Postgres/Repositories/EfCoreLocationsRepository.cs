using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище локаций на EF Core: работает через <see cref="AppDbContext"/>.
/// </summary>
public sealed class EfCoreLocationsRepository(AppDbContext dbContext) : ILocationsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<Location?> GetByIdAsync(LocationId id, CancellationToken cancellationToken) =>
        _dbContext.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public Task<bool> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken) =>
        _dbContext.Locations.AnyAsync(l => l.Name == name, cancellationToken);

    public async Task<IReadOnlyCollection<LocationId>> GetExistingIdsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken) =>
        await _dbContext.Locations
            .Where(l => locationIds.Contains(l.Id))
            .Select(l => l.Id)
            .ToArrayAsync(cancellationToken);

    public void Add(Location location) => _dbContext.Locations.Add(location);
}
