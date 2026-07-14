using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Валидация полей адреса локации.
/// </summary>
public sealed class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(a => a.City)
            .NotEmpty()
            .MaximumLength(LocationAddress.MaxCityLength);

        RuleFor(a => a.Street)
            .NotEmpty()
            .MaximumLength(LocationAddress.MaxStreetLength);

        RuleFor(a => a.House)
            .NotEmpty()
            .MaximumLength(LocationAddress.MaxHouseLength);

        RuleFor(a => a.Apartment)
            .NotEmpty()
            .MaximumLength(LocationAddress.MaxApartmentLength);
    }
}
