using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище должностей на EF Core: добавляет, загружает и удаляет сущности в рамках
/// <see cref="AppDbContext"/>. Не фиксирует изменения и не ловит технические исключения БД —
/// это ответственность <see cref="Database.TransactionManager"/>.
/// </summary>
public sealed class EfCorePositionsRepository(AppDbContext dbContext) : IPositionsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Result<Position, Failure>> GetByIdAsync(PositionId id, CancellationToken cancellationToken)
    {
        Position? position = await _dbContext.Positions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (position is null)
            return Failure.From(Error.NotFound($"Должность '{id.Value}' не найдена.", code: "directory.position.not_found"));

        return position;
    }

    public Task<bool> IsNameTakenAsync(PositionName name, CancellationToken cancellationToken) =>
        _dbContext.Positions.AnyAsync(p => p.Name == name, cancellationToken);

    public Task<bool> HasLinkedDepartmentsAsync(PositionId id, CancellationToken cancellationToken) =>
        _dbContext.DepartmentPositions.AnyAsync(dp => dp.PositionId == id, cancellationToken);

    public void Add(Position position) => _dbContext.Positions.Add(position);

    public void Remove(Position position) => _dbContext.Positions.Remove(position);
}
