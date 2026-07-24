using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища локаций.
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
    Task<Result<bool, Failure>> IsNameTakenAsync(LocationName name, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет новую локацию.
    /// </summary>
    UnitResult<Failure> Add(Location location);

    /// <summary>
    /// Возвращает те из переданных id, для которых локация существует.
    /// </summary>
    Task<Result<IReadOnlyCollection<LocationId>, Failure>> GetExistingIdsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken);
}
