using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт хранилища подразделений: добавляет, загружает и удаляет сущности в текущем контексте,
/// но не фиксирует изменения — commit остаётся за <see cref="ITransactionManager"/>.
/// </summary>
public interface IDepartmentsRepository
{
    /// <summary>
    /// Возвращает подразделение по id; <see cref="ErrorType.NotFound"/>, если оно не найдено.
    /// </summary>
    Task<Result<Department, Failure>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет подразделение вместе с его связями с локациями в текущий контекст (без сохранения).
    /// </summary>
    void Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations);

    /// <summary>
    /// Возвращает связь подразделения с локацией; <see cref="ErrorType.NotFound"/>, если связи нет.
    /// </summary>
    Task<Result<DepartmentLocation, Failure>> GetDepartmentLocationAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет связь подразделения с локацией в текущий контекст (без сохранения).
    /// </summary>
    void AddDepartmentLocation(DepartmentLocation departmentLocation);

    /// <summary>
    /// Удаляет связь подразделения с локацией из текущего контекста (без сохранения).
    /// </summary>
    void RemoveDepartmentLocation(DepartmentLocation departmentLocation);

    /// <summary>
    /// Проверяет, есть ли у подразделения дочерние подразделения.
    /// </summary>
    Task<bool> HasChildrenAsync(DepartmentId id, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет подразделение из текущего контекста (без сохранения). Связи с локациями
    /// и должностями удаляются каскадно на уровне БД в той же транзакции.
    /// </summary>
    void Remove(Department department);

    /// <summary>
    /// Возвращает связь подразделения с должностью; <see cref="ErrorType.NotFound"/>, если связи нет.
    /// </summary>
    Task<Result<DepartmentPosition, Failure>> GetDepartmentPositionAsync(
        DepartmentId departmentId,
        PositionId positionId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Добавляет связь подразделения с должностью в текущий контекст (без сохранения).
    /// </summary>
    void AddDepartmentPosition(DepartmentPosition departmentPosition);

    /// <summary>
    /// Удаляет связь подразделения с должностью из текущего контекста (без сохранения).
    /// </summary>
    void RemoveDepartmentPosition(DepartmentPosition departmentPosition);
}
