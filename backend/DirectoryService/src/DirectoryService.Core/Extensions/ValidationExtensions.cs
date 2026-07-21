using DirectoryService.Shared;
using FluentValidation.Results;

namespace DirectoryService.Core.Extensions;

/// <summary>
/// Преобразование результатов FluentValidation в доменные <see cref="Error"/>.
/// </summary>
public static class ValidationExtensions
{
    public static Error[] ToErrors(this ValidationResult validationResult) =>
        [.. validationResult.Errors.Select(failure => Error.Validation(failure.ErrorMessage, failure.PropertyName, failure.ErrorCode))];
}
