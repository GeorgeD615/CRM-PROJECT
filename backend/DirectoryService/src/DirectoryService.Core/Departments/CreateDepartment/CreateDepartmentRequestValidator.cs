using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Departments.CreateDepartment;

/// <summary>
/// Валидация запроса на создание подразделения: имя и slug переиспользуют доменные фабрики VO,
/// проверки id родителя и локаций — request-specific и отдают доменный Error.
/// </summary>
public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(r => r.CreateDepartmentDto.Name).MustBeValueObject(DepartmentName.Create);

        RuleFor(r => r.CreateDepartmentDto.Slug).MustBeValueObject(DepartmentSlug.Create);

        RuleFor(r => r.CreateDepartmentDto.ParentId)
            .NotEqual(Guid.Empty)
            .WithError(Error.Validation("Id родительского подразделения не может быть пустым.", "ParentId", "department.parent_id.empty"))
            .When(r => r.CreateDepartmentDto.ParentId.HasValue);

        RuleFor(r => r.CreateDepartmentDto.LocationIds)
            .NotNull()
            .WithError(Error.Validation("Список локаций обязателен.", "LocationIds", "department.location_ids.required"));

        RuleForEach(r => r.CreateDepartmentDto.LocationIds)
            .NotEmpty()
            .WithError(Error.Validation("Id локации не может быть пустым.", "LocationIds", "department.location_id.empty"));
    }
}
