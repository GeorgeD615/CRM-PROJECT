using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Контракт хранилища локаций. Реализация появится в инфраструктурном слое.
/// </summary>
public interface ILocationsRepository
{
    /// <summary>
    /// Проверяет, занято ли имя локации.
    /// </summary>
    Task<bool> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет новую локацию и сохраняет изменения.
    /// </summary>
    Task AddAsync(Location location, CancellationToken cancellationToken);
}
