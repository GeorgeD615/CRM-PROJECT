using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

/// <summary>
/// Хранилище подразделений на EF Core: добавляет, загружает и удаляет сущности в рамках
/// <see cref="AppDbContext"/>. Не фиксирует изменения и не ловит технические исключения БД —
/// это ответственность <see cref="Database.TransactionManager"/>.
/// </summary>
public sealed class EfCoreDepartmentsRepository(AppDbContext dbContext) : IDepartmentsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Result<Department, Failure>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        Department? department = await _dbContext.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department is null)
            return Failure.From(Error.NotFound($"Подразделение '{id.Value}' не найдено.", code: "directory.department.not_found"));

        return department;
    }

    public void Add(Department department, IReadOnlyCollection<DepartmentLocation> departmentLocations)
    {
        _dbContext.Departments.Add(department);
        _dbContext.DepartmentLocations.AddRange(departmentLocations);
    }

    public async Task<Result<DepartmentLocation, Failure>> GetDepartmentLocationAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
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

    public void AddDepartmentLocation(DepartmentLocation departmentLocation) =>
        _dbContext.DepartmentLocations.Add(departmentLocation);

    public void RemoveDepartmentLocation(DepartmentLocation departmentLocation) =>
        _dbContext.DepartmentLocations.Remove(departmentLocation);

    public Task<bool> HasChildrenAsync(DepartmentId id, CancellationToken cancellationToken) =>
        _dbContext.Departments.AnyAsync(d => d.ParentId == id, cancellationToken);

    public void Remove(Department department) => _dbContext.Departments.Remove(department);

    public async Task<Result<DepartmentPosition, Failure>> GetDepartmentPositionAsync(
        DepartmentId departmentId,
        PositionId positionId,
        CancellationToken cancellationToken)
    {
        DepartmentPosition? link = await _dbContext.DepartmentPositions.FirstOrDefaultAsync(
            dp => dp.DepartmentId == departmentId && dp.PositionId == positionId,
            cancellationToken);

        if (link is null)
            return Failure.From(Error.NotFound(
                $"Связь подразделения '{departmentId.Value}' с должностью '{positionId.Value}' не найдена.",
                code: "directory.department_position.not_found"));

        return link;
    }

    public void AddDepartmentPosition(DepartmentPosition departmentPosition) =>
        _dbContext.DepartmentPositions.Add(departmentPosition);

    public void RemoveDepartmentPosition(DepartmentPosition departmentPosition) =>
        _dbContext.DepartmentPositions.Remove(departmentPosition);
}
