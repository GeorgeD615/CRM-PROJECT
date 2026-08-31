using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Abstractions;

public interface IQuery;

public interface IValidatedQuery : IQuery;

public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
{
    Task<Result<TResponse, Failure>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
