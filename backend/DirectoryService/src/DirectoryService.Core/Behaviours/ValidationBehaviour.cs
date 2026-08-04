using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Validations;
using DirectoryService.Shared;
using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.Core.Behaviours;

/// <summary>
/// Общий прогон валидаторов команды: собирает все ошибки в один <see cref="Failure"/>,
/// либо возвращает null, если команда валидна.
/// </summary>
internal static class CommandValidator
{
    public static async Task<Failure?> ValidateAsync<TCommand>(
        IEnumerable<IValidator<TCommand>> validators,
        TCommand command,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return null;

        var context = new ValidationContext<TCommand>(command);

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
        Failure? failure = await CommandValidator.ValidateAsync(validators, command, cancellationToken);
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
        Failure? failure = await CommandValidator.ValidateAsync(validators, command, cancellationToken);
        if (failure is not null)
            return failure;

        return await inner.HandleAsync(command, cancellationToken);
    }
}
