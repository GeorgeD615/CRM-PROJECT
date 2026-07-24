using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище подразделений на EF Core: работает через <see cref="AppDbContext"/>.
/// Единственное место, где ошибки обращения к БД логируются и превращаются
/// в <see cref="ErrorType.Internal"/> без утечки деталей исключения наружу.
/// </summary>
public sealed class EfCoreDepartmentsRepository(
    AppDbContext dbContext,
    ILogger<EfCoreDepartmentsRepository> logger) : IDepartmentsRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<EfCoreDepartmentsRepository> _logger = logger;

    public async Task<Result<Department, Failure>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        try
        {
            Department? department = await _dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (department is null)
                return Failure.From(Error.NotFound($"Подразделение '{id.Value}' не найдено.", code: "directory.department.not_found"));

            return department;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load department {DepartmentId}.", id.Value);
            return Failure.From(Error.Internal("Не удалось получить подразделение."));
        }
    }

    public UnitResult<Failure> Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations)
    {
        try
        {
            _dbContext.Departments.Add(department);
            _dbContext.DepartmentLocations.AddRange(departmentLocations);

            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add department {DepartmentId}.", department.Id.Value);
            return Failure.From(Error.Internal("Не удалось добавить подразделение."));
        }
    }

    public async Task<Result<DepartmentLocation, Failure>> GetDepartmentLocationAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        try
        {
            DepartmentLocation? link = await _dbContext.DepartmentLocations.FirstOrDefaultAsync(
                dl => dl.DepartmentId == departmentId && dl.LocationId == locationId,
                cancellationToken);

            if (link is null)
                return Failure.From(Error.NotFound(
                    $"Связь подразделения '{departmentId.Value}' с локацией '{locationId.Value}' не найдена.",
                    code: "directory.department_location.not_found"));

            return link;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to load department-location link for department {DepartmentId} and location {LocationId}.",
                departmentId.Value,
                locationId.Value);
            return Failure.From(Error.Internal("Не удалось получить связь подразделения с локацией."));
        }
    }

    public UnitResult<Failure> AddDepartmentLocation(DepartmentLocation departmentLocation)
    {
        try
        {
            _dbContext.DepartmentLocations.Add(departmentLocation);

            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add department-location link {DepartmentLocationId}.", departmentLocation.Id.Value);
            return Failure.From(Error.Internal("Не удалось привязать локацию к подразделению."));
        }
    }

    public UnitResult<Failure> RemoveDepartmentLocation(DepartmentLocation departmentLocation)
    {
        try
        {
            _dbContext.DepartmentLocations.Remove(departmentLocation);

            return UnitResult.Success<Failure>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove department-location link {DepartmentLocationId}.", departmentLocation.Id.Value);
            return Failure.From(Error.Internal("Не удалось отвязать локацию от подразделения."));
        }
    }
}
