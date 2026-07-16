using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Сценарий создания локации: валидирует запрос, проверяет уникальность имени
/// и возвращает id созданной локации.
/// </summary>
public sealed class CreateLocationHandler
{
    private readonly IValidator<CreateLocationRequest> _validator;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger;

    public CreateLocationHandler(
        IValidator<CreateLocationRequest> validator,
        ILocationsRepository locationsRepository,
        ILogger<CreateLocationHandler> logger)
    {
        _validator = validator;
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<Guid> HandleAsync(CreateLocationRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

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
