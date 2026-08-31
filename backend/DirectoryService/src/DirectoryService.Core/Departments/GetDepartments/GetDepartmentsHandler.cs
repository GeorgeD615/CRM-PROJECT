using CSharpFunctionalExtensions;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.GetDepartments;

/// <summary>
/// Страница списка подразделений: фильтр, сортировка и срез страницы уходят в SQL.
/// Отфильтрованный запрос строится один раз и переиспользуется обоими обращениями к БД —
/// COUNT под текущим фильтром и сама страница, поэтому totalCount и элементы всегда согласованы.
/// </summary>
public sealed class GetDepartmentsHandler(
    IReadDbContext readDbContext,
    ILogger<GetDepartmentsHandler> logger)
    : IQueryHandler<PagedResult<DepartmentListItemDto>, GetDepartmentsQuery>
{
    public async Task<Result<PagedResult<DepartmentListItemDto>, Failure>> HandleAsync(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken)
    {
        GetDepartmentsRequest request = query.GetDepartmentsDto;

        int page = request.Page ?? GetDepartmentsOptions.DefaultPage;
        int pageSize = request.PageSize ?? GetDepartmentsOptions.DefaultPageSize;
        string sortBy = request.SortBy ?? GetDepartmentsOptions.SortByName;
        bool descending = string.Equals(
            request.SortDir ?? GetDepartmentsOptions.SortAscending,
            GetDepartmentsOptions.SortDescending,
            StringComparison.OrdinalIgnoreCase);

        IQueryable<Department> filtered = ApplySearch(readDbContext.Departments, request.Search);

        int totalCount = await filtered.CountAsync(cancellationToken);

        DepartmentListItemDto[] items = await ApplySort(filtered, sortBy, descending)
            .Select(department => new DepartmentListItemDto
            {
                Id = department.Id.Value,
                Name = department.Name.Value,
                Slug = department.Slug.Value,
                Path = department.Path.Value,
                CreatedAt = department.CreatedAt,
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation(
            "Departments page {Page} of size {PageSize} returned {Count} of {TotalCount} rows.",
            page,
            pageSize,
            items.Length,
            totalCount);

        return new PagedResult<DepartmentListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        };
    }

    // Поиск подстроки по имени без учёта регистра. Имя — value object с конвертером,
    // поэтому колонка адресуется через EF.Property, а сравнение приводится к нижнему регистру
    // на стороне БД: LOWER работает и для латиницы, и для кириллицы, и не тянет в Core
    // провайдер-специфичный ILIKE.
    private static IQueryable<Department> ApplySearch(IQueryable<Department> departments, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return departments;

        string pattern = search.Trim().ToLowerInvariant();

        return departments.Where(department =>
            EF.Property<string>(department, nameof(Department.Name)).ToLower().Contains(pattern));
    }

    // Белый список сортировок: значение из запроса выбирает готовое выражение,
    // а не подставляется в запрос само.
    private static IQueryable<Department> ApplySort(
        IQueryable<Department> departments,
        string sortBy,
        bool descending)
    {
        bool byCreatedAt = string.Equals(
            sortBy,
            GetDepartmentsOptions.SortByCreatedAt,
            StringComparison.OrdinalIgnoreCase);

        // Id как вторичный ключ сортировки: страницы не «плавают» при одинаковых значениях.
        return (byCreatedAt, descending) switch
        {
            (true, true) => departments.OrderByDescending(d => d.CreatedAt).ThenBy(d => d.Id),
            (true, false) => departments.OrderBy(d => d.CreatedAt).ThenBy(d => d.Id),
            (false, true) => departments.OrderByDescending(d => d.Name).ThenBy(d => d.Id),
            (false, false) => departments.OrderBy(d => d.Name).ThenBy(d => d.Id),
        };
    }
}
