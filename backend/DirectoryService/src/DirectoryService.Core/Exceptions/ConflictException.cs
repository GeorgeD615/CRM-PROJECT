using System.Text.Json;
using DirectoryService.Shared;

namespace DirectoryService.Core.Exceptions;

/// <summary>
/// Базовое исключение «конфликт состояния». По этому типу ExceptionMiddleware возвращает 409.
/// <see cref="Error"/>[] передаются через <see cref="Exception.Message"/> сериализованными в JSON —
/// десериализуются при перехвате исключения.
/// </summary>
public abstract class ConflictException(Error[] errors) : Exception(JsonSerializer.Serialize(errors));
