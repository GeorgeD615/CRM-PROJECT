namespace DirectoryService.Core.Locations;

/// <summary>
/// Нарушение бизнес-правила: имя локации уже занято.
/// Отличается от ошибки валидации формы (<see cref="FluentValidation.ValidationException"/>),
/// чтобы клиенту можно было отдать конфликт, а не некорректный запрос.
/// </summary>
public sealed class LocationNameAlreadyTakenException : Exception
{
    public LocationNameAlreadyTakenException(string name)
        : base($"Location with name '{name}' already exists.")
    {
        Name = name;
    }

    public string Name { get; }
}
