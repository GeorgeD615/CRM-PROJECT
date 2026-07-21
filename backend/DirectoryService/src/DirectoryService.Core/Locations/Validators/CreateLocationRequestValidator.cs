using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations.Validators;

/// <summary>
/// Валидация запроса на создание локации.
/// </summary>
public sealed class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Имя локации обязательно.")
            .MaximumLength(LocationName.MaxLength)
            .WithMessage($"Имя локации не должно превышать {LocationName.MaxLength} символов.");

        RuleFor(r => r.Address)
            .NotNull().WithMessage("Адрес локации обязателен.")
            .SetValidator(new AddressDtoValidator());
    }
}
