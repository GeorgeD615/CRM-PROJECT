using DirectoryService.Domain.Entities;

namespace DirectoryService.Core.Database;

/// <summary>
/// Контракт чтения: отдаёт только <see cref="IQueryable{T}"/> по каждой сущности и не содержит
/// операций изменения состояния (Add/Remove/Update/SaveChanges). Реализация обязана возвращать
/// запросы без tracking-а, поэтому query-handler-ам не нужно помнить про AsNoTracking.
/// </summary>
public interface IReadDbContext
{
    IQueryable<Department> Departments { get; }

    IQueryable<Location> Locations { get; }

    IQueryable<Position> Positions { get; }

    IQueryable<DepartmentLocation> DepartmentLocations { get; }

    IQueryable<DepartmentPosition> DepartmentPositions { get; }
}
