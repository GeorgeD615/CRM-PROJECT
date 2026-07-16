using DirectoryService.Core.Database;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Фиксация изменений через <see cref="AppDbContext"/>: один SaveChanges — одна транзакция.
/// </summary>
public sealed class TransactionManager : ITransactionManager
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(AppDbContext dbContext, ILogger<TransactionManager> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to save changes.");

            throw;
        }
    }
}
