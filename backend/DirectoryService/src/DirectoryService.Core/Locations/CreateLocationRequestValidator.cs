using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Валидация запроса на создание локации.
/// </summary>
public sealed class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(LocationName.MaxLength);

        RuleFor(r => r.Address)
            .NotNull()
            .SetValidator(new AddressDtoValidator());
    }
}
