using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Positions.UpdatePosition;

/// <summary>
/// Валидация запроса на переименование должности: имя переиспользует доменную фабрику VO.
/// </summary>
public sealed class UpdatePositionRequestValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionRequestValidator()
    {
        RuleFor(c => c.UpdatePositionDto.Name).MustBeValueObject(PositionName.Create);
    }
}
