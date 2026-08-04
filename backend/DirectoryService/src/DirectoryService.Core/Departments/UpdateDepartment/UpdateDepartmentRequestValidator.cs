using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Departments.UpdateDepartment;

/// <summary>
/// Валидация запроса на обновление подразделения: имя переиспользует доменную фабрику VO.
/// </summary>
public sealed class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(r => r.UpdateDepartmentDto.Name).MustBeValueObject(DepartmentName.Create);
    }
}
