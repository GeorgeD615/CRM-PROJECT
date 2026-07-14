using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Бизнес-сценарии работы с локациями.
/// </summary>
public sealed class LocationsService
{
    private readonly IValidator<CreateLocationRequest> _createValidator;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<LocationsService> _logger;

    public LocationsService(
        IValidator<CreateLocationRequest> createValidator,
        ILocationsRepository locationsRepository,
        ILogger<LocationsService> logger)
    {
        _createValidator = createValidator;
        _locationsRepository = locationsRepository;
        _logger = logger;
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
        {
            _logger.LogWarning("Location name {LocationName} is already taken.", name.Value);

            throw new LocationNameAlreadyTakenException(name.Value);
        }

        var location = Location.Create(name, address);
        Guid id = location.Id.Value;

        await _locationsRepository.AddAsync(location, cancellationToken);

        _logger.LogInformation("Location {LocationId} created.", id);

        return id;
    }
}
