using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments.Validators;

/// <summary>
/// Валидация запроса на создание подразделения.
/// </summary>
public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Имя подразделения обязательно.")
            .MaximumLength(DepartmentName.MaxLength)
            .WithMessage($"Имя подразделения не должно превышать {DepartmentName.MaxLength} символов.");

        RuleFor(r => r.Slug)
            .NotEmpty().WithMessage("Slug подразделения обязателен.")
            .Length(DepartmentSlug.MinLength, DepartmentSlug.MaxLength)
            .WithMessage($"Slug должен быть от {DepartmentSlug.MinLength} до {DepartmentSlug.MaxLength} символов.");

        RuleFor(r => r.ParentId)
            .NotEqual(Guid.Empty).WithMessage("Id родительского подразделения не может быть пустым.")
            .When(r => r.ParentId.HasValue);

        RuleFor(r => r.LocationIds)
            .NotNull().WithMessage("Список локаций обязателен.");

        RuleForEach(r => r.LocationIds)
            .NotEmpty().WithMessage("Id локации не может быть пустым.");
    }
}
