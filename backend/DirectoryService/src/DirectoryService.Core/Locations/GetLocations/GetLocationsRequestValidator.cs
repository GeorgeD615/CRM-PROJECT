using DirectoryService.Core.Validations;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Locations.GetLocations;

/// <summary>
/// Валидация параметров списка локаций: страница, размер страницы, длина поиска,
/// минимальное число подразделений и допустимые значения сортировки. Неизвестное поле
/// сортировки — валидационная ошибка (400), а не строка, попавшая в ORDER BY.
/// </summary>
public sealed class GetLocationsRequestValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsRequestValidator()
    {
        RuleFor(q => q.GetLocationsDto.Search)
            .MaximumLength(GetLocationsOptions.MaxSearchLength)
            .WithError(Error.Validation(
                $"Поисковая строка не должна превышать {GetLocationsOptions.MaxSearchLength} символов.",
                "Search",
                "directory.location.search.too_long"));

        RuleFor(q => q.GetLocationsDto.MinDepartmentCount)
            .GreaterThanOrEqualTo(0)
            .When(q => q.GetLocationsDto.MinDepartmentCount.HasValue)
            .WithError(Error.Validation(
                "Минимальное число подразделений не может быть отрицательным.",
                "MinDepartmentCount",
                "directory.location.min_department_count.invalid"));

        RuleFor(q => q.GetLocationsDto.Page)
            .GreaterThanOrEqualTo(GetLocationsOptions.DefaultPage)
            .When(q => q.GetLocationsDto.Page.HasValue)
            .WithError(Error.Validation(
                "Номер страницы начинается с 1.",
                "Page",
                "directory.location.page.invalid"));

        RuleFor(q => q.GetLocationsDto.PageSize)
            .InclusiveBetween(1, GetLocationsOptions.MaxPageSize)
            .When(q => q.GetLocationsDto.PageSize.HasValue)
            .WithError(Error.Validation(
                $"Размер страницы должен быть от 1 до {GetLocationsOptions.MaxPageSize}.",
                "PageSize",
                "directory.location.page_size.invalid"));

        RuleFor(q => q.GetLocationsDto.SortBy)
            .Must(sortBy => GetLocationsOptions.IsKnownSortField(sortBy!))
            .When(q => !string.IsNullOrWhiteSpace(q.GetLocationsDto.SortBy))
            .WithError(Error.Validation(
                $"Сортировка возможна только по полям: {string.Join(", ", GetLocationsOptions.SortFields)}.",
                "SortBy",
                "directory.location.sort_by.unknown"));

        RuleFor(q => q.GetLocationsDto.SortDir)
            .Must(sortDir => GetLocationsOptions.IsKnownSortDirection(sortDir!))
            .When(q => !string.IsNullOrWhiteSpace(q.GetLocationsDto.SortDir))
            .WithError(Error.Validation(
                $"Направление сортировки может быть только {GetLocationsOptions.SortAscending} или {GetLocationsOptions.SortDescending}.",
                "SortDir",
                "directory.location.sort_dir.unknown"));
    }
}
