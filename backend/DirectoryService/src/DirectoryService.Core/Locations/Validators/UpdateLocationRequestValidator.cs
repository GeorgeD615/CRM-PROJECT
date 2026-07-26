using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Validations;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Locations.Validators;

/// <summary>
/// Валидация запроса на обновление локации: правила имени и адреса переиспользуют доменные фабрики VO.
/// </summary>
public sealed class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
{
    public UpdateLocationRequestValidator()
    {
        RuleFor(r => r.Name).MustBeValueObject(LocationName.Create);

        RuleFor(r => r.Address)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithError(Error.Validation("Адрес локации обязателен.", "Address", "location.address.required"))
            .MustBeValueObject(address => LocationAddress.Create(address.City, address.Street, address.House, address.Apartment));
    }
}
