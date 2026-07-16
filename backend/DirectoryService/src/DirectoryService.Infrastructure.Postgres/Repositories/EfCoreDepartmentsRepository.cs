using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище подразделений на EF Core: работает через <see cref="AppDbContext"/>.
/// </summary>
public sealed class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EfCoreDepartmentsRepository> _logger;

    public EfCoreDepartmentsRepository(AppDbContext dbContext, ILogger<EfCoreDepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken) =>
        _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Departments.Add(department);
            _dbContext.DepartmentLocations.AddRange(departmentLocations);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to save department {DepartmentId} with its locations.",
                department.Id.Value);

            throw;
        }
    }
}
