using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Валидация запроса на создание подразделения.
/// </summary>
public sealed class CreateDepartmentRequestValidator : AbstractValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(DepartmentName.MaxLength);

        RuleFor(r => r.Slug)
            .NotEmpty()
            .Length(DepartmentSlug.MinLength, DepartmentSlug.MaxLength);

        RuleFor(r => r.ParentId)
            .NotEqual(Guid.Empty)
            .When(r => r.ParentId.HasValue);

        RuleFor(r => r.LocationIds)
            .NotNull();

        RuleForEach(r => r.LocationIds)
            .NotEmpty();
    }
}
