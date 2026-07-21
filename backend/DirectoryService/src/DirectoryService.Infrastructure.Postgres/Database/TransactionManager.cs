using DirectoryService.Core.Database;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Фиксация изменений через <see cref="AppDbContext"/>: один SaveChanges — одна транзакция.
/// </summary>
public sealed class TransactionManager(AppDbContext dbContext) : ITransactionManager
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
