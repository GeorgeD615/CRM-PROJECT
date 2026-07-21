using System.Text.Json;
using DirectoryService.Shared;

namespace DirectoryService.Core.Exceptions;

/// <summary>
/// Базовое исключение «некорректный запрос». По этому типу ExceptionMiddleware возвращает 400.
/// <see cref="ValidationException"/> — его частный случай. <see cref="Error"/>[] передаются через
/// <see cref="Exception.Message"/> сериализованными в JSON — десериализуются при перехвате исключения.
/// </summary>
public abstract class BadRequestException(Error[] errors) : Exception(JsonSerializer.Serialize(errors));
