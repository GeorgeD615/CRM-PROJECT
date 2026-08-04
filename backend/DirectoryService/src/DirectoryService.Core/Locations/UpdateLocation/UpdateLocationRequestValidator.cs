
using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Locations.UpdateLocation;

/// <summary>
/// Валидация запроса на обновление локации: правила имени и адреса переиспользуют доменные фабрики VO.
/// </summary>
public sealed class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationRequestValidator()
    {
        RuleFor(r => r.UpdateLocationDto.Name).MustBeValueObject(LocationName.Create);

        RuleFor(r => r.UpdateLocationDto.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithError(Error.Validation("Адрес локации обязателен.", "Address", "location.address.required"))
            .MustBeValueObject(address => LocationAddress.Create(address.City, address.Street, address.House, address.Apartment));
    }
}
