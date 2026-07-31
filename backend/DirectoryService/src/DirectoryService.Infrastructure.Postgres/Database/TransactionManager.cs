using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Единая граница сохранения: <see cref="SaveChangesAsync"/> — обычный commit для простых сценариев,
/// <see cref="BeginTransactionAsync"/> — явная транзакция на время use case-а с несколькими зависимыми
/// изменениями. Технические исключения БД ловятся здесь, категоризируются через <see cref="DbExceptionMapper"/>
/// и возвращаются доменным <see cref="Error"/>, наружу детали не утекают.
/// </summary>
public sealed class TransactionManager(
    AppDbContext dbContext,
    ILogger<TransactionManager> logger,
    ILoggerFactory loggerFactory) : ITransactionManager
{
    public async Task<Result<ITransactionScope, Failure>> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            var transactionScope = new TransactionScope(
                loggerFactory.CreateLogger<TransactionScope>(),
                transaction.GetDbTransaction());

            return transactionScope;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to begin transaction.");
            return Failure.FromError(Error.Internal("Не удалось начать транзакцию.", code: "database.transaction.failed"));
        }
    }

    public async Task<UnitResult<Failure>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            Failure failure = DbExceptionMapper.Map(ex);
            LogDbFailure(ex, failure);

            return failure;
        }
    }

    private void LogDbFailure(Exception exception, Failure failure)
    {
        if (DbExceptionMapper.IsTechnical(failure))
            logger.LogError(exception, "Database failure while saving changes.");
        else
            logger.LogWarning(exception, "Database constraint conflict while saving changes.");
    }
}
