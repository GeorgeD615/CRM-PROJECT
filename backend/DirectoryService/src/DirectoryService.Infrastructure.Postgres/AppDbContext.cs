using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IReadDbContext
{
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

    public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();

    // Явная реализация read-контракта: AsNoTracking зашит здесь, чтобы ни один
    // query-handler не мог случайно получить отслеживаемый запрос.
    IQueryable<Department> IReadDbContext.Departments => Set<Department>().AsNoTracking();

    IQueryable<Location> IReadDbContext.Locations => Set<Location>().AsNoTracking();

    IQueryable<Position> IReadDbContext.Positions => Set<Position>().AsNoTracking();

    IQueryable<DepartmentLocation> IReadDbContext.DepartmentLocations => Set<DepartmentLocation>().AsNoTracking();

    IQueryable<DepartmentPosition> IReadDbContext.DepartmentPositions => Set<DepartmentPosition>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
