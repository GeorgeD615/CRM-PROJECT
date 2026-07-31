using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище локаций на EF Core: добавляет и загружает сущности в рамках <see cref="AppDbContext"/>.
/// Не фиксирует изменения и не ловит технические исключения БД — это ответственность
/// <see cref="Database.TransactionManager"/>.
/// </summary>
public sealed class EfCoreLocationsRepository(AppDbContext dbContext) : ILocationsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Result<Location, Failure>> GetByIdAsync(LocationId id, CancellationToken cancellationToken)
    {
        Location? location = await _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (location is null)
            return Failure.From(Error.NotFound($"Локация '{id.Value}' не найдена.", code: "directory.location.not_found"));

        return location;
    }

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
