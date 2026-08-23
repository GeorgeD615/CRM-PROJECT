using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.GetDepartmentById;

/// <summary>
/// Сценарий чтения карточки подразделения: собирает DTO прямо в запросе, поэтому в БД уходит
/// SELECT по нужным колонкам, а доменная сущность не материализуется. Работает через
/// <see cref="IReadDbContext"/> — без tracking-а и без доступа к операциям записи.
/// </summary>
public sealed class GetDepartmentByIdHandler(
    IReadDbContext readDbContext,
    ILogger<GetDepartmentByIdHandler> logger) : IQueryHandler<GetDepartmentDto, GetDepartmentByIdQuery>
{

    public async Task<Result<GetDepartmentDto, Failure>> HandleAsync(
        GetDepartmentByIdQuery query,
        CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(query.DepartmentId);

        GetDepartmentDto? department = await readDbContext.Departments
            .Where(d => d.Id == departmentId)
            .Select(d => new GetDepartmentDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Slug = d.Slug.Value,
                Path = d.Path.Value,
                ParentId = d.ParentId != null ? (Guid?)d.ParentId.Value : null,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            logger.LogWarning("Department {DepartmentId} not found.", query.DepartmentId);

            return Failure.From(Error.NotFound(
                $"Подразделение '{query.DepartmentId}' не найдено.",
                code: "directory.department.not_found"));
        }

        return department;
    }
}
