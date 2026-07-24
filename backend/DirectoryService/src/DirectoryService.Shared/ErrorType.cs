namespace DirectoryService.Shared;

/// <summary>
/// Тип ошибки
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Отсутствие ошибки
    /// </summary>
    None = 0,

    /// <summary>
    /// Ошибка валидации
    /// </summary>
    Validation,

    /// <summary>
    /// Ошибка поиска
    /// </summary>
    NotFound,

    /// <summary>
    /// Конфликт
    /// </summary>
    Conflict,

    /// <summary>
    /// Внутренняя ошибка
    /// </summary>
    Internal,
}
