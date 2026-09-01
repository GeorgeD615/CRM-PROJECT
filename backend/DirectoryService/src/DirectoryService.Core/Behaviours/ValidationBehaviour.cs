using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Validations;
using DirectoryService.Shared;
using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.Core.Behaviours;

/// <summary>
/// Общий прогон валидаторов входящего запроса (команды или query): собирает все ошибки
/// в один <see cref="Failure"/>, либо возвращает null, если запрос валиден.
/// </summary>
internal static class RequestValidator
{
    public static async Task<Failure?> ValidateAsync<TRequest>(
        IEnumerable<IValidator<TRequest>> validators,
        TRequest command,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return null;

        var context = new ValidationContext<TRequest>(command);

        ValidationResult[] results = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        Error[] errors = [.. results
            .Where(result => !result.IsValid)
            .SelectMany(result => result.ToErrors())];

        return errors.Length > 0 ? new Failure(errors) : null;
    }
}

/// <summary>
/// Валидация как cross-cutting decorator для команд, возвращающих данные
/// (<see cref="ICommandHandler{TResponse, TCommand}"/>). Невалидная команда до handler-а не доходит.
/// </summary>
internal sealed class ValidationBehaviour<TResponse, TCommand>(
    IEnumerable<IValidator<TCommand>> validators,
    ICommandHandler<TResponse, TCommand> inner) : ICommandHandler<TResponse, TCommand>
    where TCommand : IValidatedCommand
{
    public async Task<Result<TResponse, Failure>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        Failure? failure = await RequestValidator.ValidateAsync(validators, command, cancellationToken);
        if (failure is not null)
            return failure;

        return await inner.HandleAsync(command, cancellationToken);
    }
}

/// <summary>
/// Валидация как cross-cutting decorator для команд без данных
/// (<see cref="ICommandHandler{TCommand}"/>). Невалидная команда до handler-а не доходит.
/// </summary>
internal sealed class ValidationBehaviour<TCommand>(
    IEnumerable<IValidator<TCommand>> validators,
    ICommandHandler<TCommand> inner) : ICommandHandler<TCommand>
    where TCommand : IValidatedCommand
{
    public async Task<UnitResult<Failure>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        Failure? failure = await RequestValidator.ValidateAsync(validators, command, cancellationToken);
        if (failure is not null)
            return failure;

        return await inner.HandleAsync(command, cancellationToken);
    }
}

/// <summary>
/// Та же валидация для сценариев чтения (<see cref="IQueryHandler{TResponse, TQuery}"/>):
/// параметры списка приходят от клиента, поэтому проверяются до того, как попадут в LINQ.
/// </summary>
internal sealed class QueryValidationBehaviour<TResponse, TQuery>(
    IEnumerable<IValidator<TQuery>> validators,
    IQueryHandler<TResponse, TQuery> inner) : IQueryHandler<TResponse, TQuery>
    where TQuery : IValidatedQuery
{
    public async Task<Result<TResponse, Failure>> HandleAsync(TQuery query, CancellationToken cancellationToken)
    {
        Failure? failure = await RequestValidator.ValidateAsync(validators, query, cancellationToken);
        if (failure is not null)
            return failure;

        return await inner.HandleAsync(query, cancellationToken);
    }
}
