using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Бизнес-сценарии работы с локациями.
/// </summary>
public sealed class LocationsService
{
    private readonly IValidator<CreateLocationRequest> _createValidator;
    private readonly ILocationsRepository _locationsRepository;

    public LocationsService(
        IValidator<CreateLocationRequest> createValidator,
        ILocationsRepository locationsRepository)
    {
        _createValidator = createValidator;
        _locationsRepository = locationsRepository;
    }

    /// <summary>
    /// Создаёт локацию: валидирует запрос, проверяет уникальность имени
    /// и возвращает id созданной локации.
    /// </summary>
    public async Task<Guid> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var name = LocationName.Create(request.Name);
        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment);

        if (await _locationsRepository.IsNameTakenAsync(name, cancellationToken))
            throw new LocationNameAlreadyTakenException(name.Value);

        var location = Location.Create(name, address);

        await _locationsRepository.AddAsync(location, cancellationToken);

        return location.Id.Value;
    }
}
