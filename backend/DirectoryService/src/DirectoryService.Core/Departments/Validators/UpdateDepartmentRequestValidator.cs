using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments.Validators;

/// <summary>
/// Валидация запроса на обновление подразделения.
/// </summary>
public sealed class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Имя подразделения обязательно.")
            .MaximumLength(DepartmentName.MaxLength)
            .WithMessage($"Имя подразделения не должно превышать {DepartmentName.MaxLength} символов.");
    }
}
