using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Сценарий обновления локации: меняет имя и адрес существующей локации,
/// не допуская дубля имени с другой локацией.
/// </summary>
public sealed class UpdateLocationHandler
{
    private readonly IValidator<UpdateLocationRequest> _validator;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<UpdateLocationHandler> _logger;

    public UpdateLocationHandler(
        IValidator<UpdateLocationRequest> validator,
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        ILogger<UpdateLocationHandler> logger)
    {
        _validator = validator;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task HandleAsync(
        Guid locationId,
        UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Location? location = await _locationsRepository.GetByIdAsync(
            LocationId.Create(locationId),
            cancellationToken);

        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} does not exist.", locationId);

            throw new LocationNotFoundException(locationId);
        }

        var name = LocationName.Create(request.Name);

        if (name != location.Name && await _locationsRepository.IsNameTakenAsync(name, cancellationToken))
        {
            _logger.LogWarning("Location name {LocationName} is already taken.", name.Value);

            throw new LocationNameAlreadyTakenException(name.Value);
        }

        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment);

        location.Rename(name);
        location.ChangeAddress(address);

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Location {LocationId} updated.", locationId);
    }
}
