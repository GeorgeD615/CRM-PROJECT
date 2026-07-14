using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища локаций. Реализации живут в инфраструктурном слое.
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
