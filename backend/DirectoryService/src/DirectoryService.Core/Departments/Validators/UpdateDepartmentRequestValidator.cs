using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments.Validators;

/// <summary>
/// Валидация запроса на обновление подразделения: имя переиспользует доменную фабрику VO.
/// </summary>
public sealed class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(r => r.Name).MustBeValueObject(DepartmentName.Create);
    }
}
