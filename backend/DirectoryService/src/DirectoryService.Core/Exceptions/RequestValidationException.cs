using DirectoryService.Shared;

namespace DirectoryService.Core.Exceptions;

/// <summary>
/// Ошибки валидации входящего запроса, собранные из результата FluentValidation.
/// </summary>
public sealed class RequestValidationException(Failure failure) : ValidationException([.. failure]);
