using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Locations.Exceptions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Сценарий создания локации: валидирует запрос, проверяет уникальность имени
/// и возвращает id созданной локации.
/// </summary>
public sealed class CreateLocationHandler(
    IValidator<CreateLocationRequest> validator,
    ILocationsRepository locationsRepository,
    ILogger<CreateLocationHandler> logger)
{
    private readonly IValidator<CreateLocationRequest> _validator = validator;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ILogger<CreateLocationHandler> _logger = logger;

    public async Task<Guid> HandleAsync(CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new RequestValidationException(validationResult.ToErrors());

        var name = LocationName.Create(request.Name);
        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment);

        if (await _locationsRepository.IsNameTakenAsync(name, cancellationToken))
            throw new LocationNameAlreadyTakenException(name.Value);

        var location = Location.Create(name, address);
        Guid id = location.Id.Value;

        await _locationsRepository.AddAsync(location, cancellationToken);

        _logger.LogInformation("Location {LocationId} created.", id);

        return id;
    }
}
