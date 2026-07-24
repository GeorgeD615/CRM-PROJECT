using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища подразделений.
/// </summary>
public interface IDepartmentsRepository
{
    /// <summary>
    /// Возвращает подразделение по id; <see cref="ErrorType.NotFound"/>, если оно не найдено.
    /// </summary>
    Task<Result<Department, Failure>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет подразделение вместе с его связями с локациями.
    /// </summary>
    UnitResult<Failure> Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations);

    /// <summary>
    /// Возвращает связь подразделения с локацией; <see cref="ErrorType.NotFound"/>, если связи нет.
    /// </summary>
    Task<Result<DepartmentLocation, Failure>> GetDepartmentLocationAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет связь подразделения с локацией.
    /// </summary>
    UnitResult<Failure> AddDepartmentLocation(DepartmentLocation departmentLocation);

    /// <summary>
    /// Удаляет связь подразделения с локацией.
    /// </summary>
    UnitResult<Failure> RemoveDepartmentLocation(DepartmentLocation departmentLocation);
}
