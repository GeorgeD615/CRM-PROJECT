using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Failure> Commit();

    UnitResult<Failure> Rollback();
}
