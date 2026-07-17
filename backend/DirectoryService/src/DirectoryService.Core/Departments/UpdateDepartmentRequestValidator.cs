using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Валидация запроса на обновление подразделения.
/// </summary>
public sealed class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(DepartmentName.MaxLength);
    }
}
