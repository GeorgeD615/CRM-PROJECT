using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Failure>> BeginTransactionAsync(CancellationToken cancellationToken);
    Task<UnitResult<Failure>> SaveChangesAsync(CancellationToken cancellationToken);
}