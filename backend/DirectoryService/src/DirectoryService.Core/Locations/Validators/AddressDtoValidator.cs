using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations.Validators;

/// <summary>
/// Валидация полей адреса локации.
/// </summary>
public sealed class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(a => a.City)
            .NotEmpty().WithMessage("Город обязателен.")
            .MaximumLength(LocationAddress.MaxCityLength)
            .WithMessage($"Город не должен превышать {LocationAddress.MaxCityLength} символов.");

        RuleFor(a => a.Street)
            .NotEmpty().WithMessage("Улица обязательна.")
            .MaximumLength(LocationAddress.MaxStreetLength)
            .WithMessage($"Улица не должна превышать {LocationAddress.MaxStreetLength} символов.");

        RuleFor(a => a.House)
            .NotEmpty().WithMessage("Дом обязателен.")
            .MaximumLength(LocationAddress.MaxHouseLength)
            .WithMessage($"Номер дома не должен превышать {LocationAddress.MaxHouseLength} символов.");

        RuleFor(a => a.Apartment)
            .NotEmpty().WithMessage("Квартира обязательна.")
            .MaximumLength(LocationAddress.MaxApartmentLength)
            .WithMessage($"Номер квартиры не должен превышать {LocationAddress.MaxApartmentLength} символов.");
    }
}
