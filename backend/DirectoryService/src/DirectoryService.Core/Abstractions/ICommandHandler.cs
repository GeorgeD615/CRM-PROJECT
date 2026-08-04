using CSharpFunctionalExtensions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Abstractions;

public interface ICommand;

public interface IValidatedCommand : ICommand;

public interface ICommandHandler<TResponse, in TCommand>
    where TCommand : ICommand
{
    Task<Result<TResponse, Failure>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Failure>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
