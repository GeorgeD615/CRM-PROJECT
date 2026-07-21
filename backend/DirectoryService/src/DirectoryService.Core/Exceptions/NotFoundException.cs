using System.Text.Json;
using DirectoryService.Shared;

namespace DirectoryService.Core.Exceptions;

/// <summary>
/// Базовое исключение «сущность не найдена». По этому типу ExceptionMiddleware возвращает 404.
/// <see cref="Error"/>[] передаются через <see cref="Exception.Message"/> сериализованными в JSON —
/// десериализуются при перехвате исключения.
/// </summary>
public abstract class NotFoundException(Error[] errors) : Exception(JsonSerializer.Serialize(errors));
