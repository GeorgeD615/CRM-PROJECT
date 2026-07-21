using DirectoryService.Shared;

namespace DirectoryService.Core.Exceptions;

/// <summary>
/// Ошибка валидации бизнес-данных
/// </summary>
public abstract class ValidationException(Error[] errors) : BadRequestException(errors);
