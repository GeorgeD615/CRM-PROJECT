using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Positions.CreatePosition;

/// <summary>
/// Валидация запроса на создание должности: имя переиспользует доменную фабрику VO.
/// </summary>
public sealed class CreatePositionRequestValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionRequestValidator()
    {
        RuleFor(c => c.CreatePositionDto.Name).MustBeValueObject(PositionName.Create);
    }
}
