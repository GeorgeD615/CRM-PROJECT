using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище подразделений на EF Core: работает через <see cref="AppDbContext"/>.
/// </summary>
public sealed class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly AppDbContext _dbContext;

    public EfCoreDepartmentsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken) =>
        _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations)
    {
        _dbContext.Departments.Add(department);
        _dbContext.DepartmentLocations.AddRange(departmentLocations);
    }
}
