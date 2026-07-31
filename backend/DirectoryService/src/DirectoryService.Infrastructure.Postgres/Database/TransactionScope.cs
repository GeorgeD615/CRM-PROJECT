using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Database;

/// <summary>
/// Явная транзакция на время use case-а. Commit категоризирует технические сбои через
/// <see cref="DbExceptionMapper"/>; при Dispose незакоммиченная транзакция откатывается.
/// </summary>
public sealed class TransactionScope(ILogger<TransactionScope> logger, IDbTransaction transaction) : ITransactionScope
{
    public UnitResult<Failure> Commit()
    {
        try
        {
            transaction.Commit();
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            Failure failure = DbExceptionMapper.Map(ex);

            if (DbExceptionMapper.IsTechnical(failure))
                logger.LogError(ex, "Failed to commit transaction.");
            else
                logger.LogWarning(ex, "Transaction commit conflicted.");

            return failure;
        }
    }

    public UnitResult<Failure> Rollback()
    {
        try
        {
            transaction.Rollback();
            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback transaction.");
            return Failure.FromError(Error.Internal("Не удалось откатить транзакцию.", code: "database.transaction.rollback"));
        }
    }

    public void Dispose() => transaction.Dispose();
}
