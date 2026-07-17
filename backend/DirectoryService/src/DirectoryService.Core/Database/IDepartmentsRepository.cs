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
    /// Добавляет подразделение вместе с его связями с локациями.
    /// </summary>
    void Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations);

    /// <summary>
    /// Возвращает связь подразделения с локацией или null, если связи нет.
    /// </summary>
    Task<DepartmentLocation?> GetDepartmentLocationAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет связь подразделения с локацией.
    /// </summary>
    void AddDepartmentLocation(DepartmentLocation departmentLocation);

    /// <summary>
    /// Удаляет связь подразделения с локацией.
    /// </summary>
    void RemoveDepartmentLocation(DepartmentLocation departmentLocation);
}
