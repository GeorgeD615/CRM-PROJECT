using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища подразделений.
/// </summary>
public interface IDepartmentsRepository
{
    /// <summary>
    /// Возвращает подразделение по id или null, если оно не найдено.
    /// </summary>
    Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет подразделение вместе с его связями с локациями
    /// </summary>
    Task AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken);
}
