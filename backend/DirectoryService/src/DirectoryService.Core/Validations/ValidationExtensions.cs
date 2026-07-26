using DirectoryService.Shared;
using FluentValidation.Results;

namespace DirectoryService.Core.Validations;

/// <summary>
/// Единый путь ValidationResult → <see cref="Failure"/>: восстанавливает доменные <see cref="Error"/>,
/// зашитые в сообщения (см. <c>CustomValidators</c>), сохраняя их code. Все ошибки, а не только первая.
/// </summary>
public static class ValidationExtensions
{
    public static Failure ToErrors(this ValidationResult validationResult)
    {
        Error[] errors = [.. validationResult.Errors.Select(ToError)];

        return errors;
    }

    private static Error ToError(ValidationFailure failure) =>
        Error.TryDeserialize(failure.ErrorMessage, out Error? domainError)
            ? Error.Validation(domainError.Message, failure.PropertyName, domainError.Code)
            : Error.Validation(failure.ErrorMessage, failure.PropertyName);
}
