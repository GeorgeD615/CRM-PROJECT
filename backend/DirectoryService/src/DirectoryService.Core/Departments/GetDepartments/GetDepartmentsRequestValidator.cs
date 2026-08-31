using DirectoryService.Core.Validations;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Departments.GetDepartments;

/// <summary>
/// Валидация параметров списка подразделений: страница, размер страницы, длина поиска
/// и допустимые значения сортировки. Опечатка в sortBy — валидационная ошибка (400),
/// а не падение при построении запроса.
/// </summary>
public sealed class GetDepartmentsRequestValidator : AbstractValidator<GetDepartmentsQuery>
{
    public GetDepartmentsRequestValidator()
    {
        RuleFor(q => q.GetDepartmentsDto.Search)
            .MaximumLength(GetDepartmentsOptions.MaxSearchLength)
            .WithError(Error.Validation(
                $"Поисковая строка не должна превышать {GetDepartmentsOptions.MaxSearchLength} символов.",
                "Search",
                "directory.department.search.too_long"));

        RuleFor(q => q.GetDepartmentsDto.Page)
            .GreaterThanOrEqualTo(GetDepartmentsOptions.DefaultPage)
            .When(q => q.GetDepartmentsDto.Page.HasValue)
            .WithError(Error.Validation(
                "Номер страницы начинается с 1.",
                "Page",
                "directory.department.page.invalid"));

        RuleFor(q => q.GetDepartmentsDto.PageSize)
            .InclusiveBetween(1, GetDepartmentsOptions.MaxPageSize)
            .When(q => q.GetDepartmentsDto.PageSize.HasValue)
            .WithError(Error.Validation(
                $"Размер страницы должен быть от 1 до {GetDepartmentsOptions.MaxPageSize}.",
                "PageSize",
                "directory.department.page_size.invalid"));

        RuleFor(q => q.GetDepartmentsDto.SortBy)
            .Must(sortBy => GetDepartmentsOptions.IsKnownSortField(sortBy!))
            .When(q => !string.IsNullOrWhiteSpace(q.GetDepartmentsDto.SortBy))
            .WithError(Error.Validation(
                $"Сортировка возможна только по полям: {string.Join(", ", GetDepartmentsOptions.SortFields)}.",
                "SortBy",
                "directory.department.sort_by.unknown"));

        RuleFor(q => q.GetDepartmentsDto.SortDir)
            .Must(sortDir => GetDepartmentsOptions.IsKnownSortDirection(sortDir!))
            .When(q => !string.IsNullOrWhiteSpace(q.GetDepartmentsDto.SortDir))
            .WithError(Error.Validation(
                $"Направление сортировки может быть только {GetDepartmentsOptions.SortAscending} или {GetDepartmentsOptions.SortDescending}.",
                "SortDir",
                "directory.department.sort_dir.unknown"));
    }
}
