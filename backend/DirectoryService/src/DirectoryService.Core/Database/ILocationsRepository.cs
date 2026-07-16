using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища локаций.
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

    /// <summary>
    /// Возвращает те из переданных id, для которых локация существует.
    /// </summary>
    Task<IReadOnlyCollection<LocationId>> GetExistingIdsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken);
}
