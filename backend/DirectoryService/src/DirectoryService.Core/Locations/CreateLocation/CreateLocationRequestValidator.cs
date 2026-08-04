using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Locations.CreateLocation;

/// <summary>
/// Валидация запроса на создание локации: правила имени и адреса переиспользуют доменные фабрики VO.
/// </summary>
public sealed class CreateLocationRequestValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationRequestValidator()
    {
        RuleFor(r => r.CreateLocationDto.Name).MustBeValueObject(LocationName.Create);

        RuleFor(r => r.CreateLocationDto.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithError(Error.Validation("Адрес локации обязателен.", "Address", "location.address.required"))
            .MustBeValueObject(address => LocationAddress.Create(address.City, address.Street, address.House, address.Apartment));
    }
}
