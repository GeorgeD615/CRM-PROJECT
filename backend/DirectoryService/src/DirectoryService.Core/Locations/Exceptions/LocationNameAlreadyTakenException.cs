using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Locations.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: имя локации уже занято.
/// Отличается от ошибки валидации формы, чтобы клиенту можно было отдать конфликт,
/// а не некорректный запрос.
/// </summary>
public sealed class LocationNameAlreadyTakenException(string name) :
    ConflictException([Error.Conflict($"Локация с именем '{name}' уже существует.")]);
