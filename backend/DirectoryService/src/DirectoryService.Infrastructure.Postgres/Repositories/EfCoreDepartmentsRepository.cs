using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище подразделений на EF Core: работает через <see cref="AppDbContext"/>.
/// </summary>
public sealed class EfCoreDepartmentsRepository(AppDbContext dbContext) : IDepartmentsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken) =>
        _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations)
    {
        _dbContext.Departments.Add(department);
        _dbContext.DepartmentLocations.AddRange(departmentLocations);
    }

    public Task<DepartmentLocation?> GetDepartmentLocationAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken) =>
        _dbContext.DepartmentLocations.FirstOrDefaultAsync(
            dl => dl.DepartmentId == departmentId && dl.LocationId == locationId,
            cancellationToken);

    public void AddDepartmentLocation(DepartmentLocation departmentLocation) =>
        _dbContext.DepartmentLocations.Add(departmentLocation);

    public void RemoveDepartmentLocation(DepartmentLocation departmentLocation) =>
        _dbContext.DepartmentLocations.Remove(departmentLocation);
}
