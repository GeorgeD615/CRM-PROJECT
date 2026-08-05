using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища локаций: добавляет и загружает сущности в текущем контексте,
/// но не фиксирует изменения — commit остаётся за <see cref="ITransactionManager"/>.
/// </summary>
public interface ILocationsRepository
{
    /// <summary>
    /// Возвращает локацию по id; <see cref="ErrorType.NotFound"/>, если она не найдена.
    /// </summary>
    Task<Result<Location, Failure>> GetByIdAsync(LocationId id, CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, занято ли имя локации.
    /// </summary>
    Task<bool> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет новую локацию в текущий контекст (без сохранения).
    /// </summary>
    void Add(Location location);

    /// <summary>
    /// Возвращает те из переданных id, для которых локация существует.
    /// </summary>
    Task<IReadOnlyCollection<LocationId>> GetExistingIdsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken);

    /// <summary>
    /// Проверяет, привязана ли локация хотя бы к одному подразделению.
    /// </summary>
    Task<bool> HasLinkedDepartmentsAsync(LocationId id, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет локацию из текущего контекста (без сохранения).
    /// </summary>
    void Remove(Location location);
}
