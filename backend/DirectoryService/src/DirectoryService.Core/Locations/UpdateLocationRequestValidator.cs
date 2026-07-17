using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Валидация запроса на обновление локации.
/// </summary>
public sealed class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
{
    public UpdateLocationRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(LocationName.MaxLength);

        RuleFor(r => r.Address)
            .NotNull()
            .SetValidator(new AddressDtoValidator());
    }
}
