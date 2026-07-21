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
/// Сценарий обновления локации: меняет имя и адрес существующей локации,
/// не допуская дубля имени с другой локацией.
/// </summary>
public sealed class UpdateLocationHandler(
    IValidator<UpdateLocationRequest> validator,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateLocationHandler> logger)
{
    private readonly IValidator<UpdateLocationRequest> _validator = validator;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<UpdateLocationHandler> _logger = logger;

    public async Task HandleAsync(
        Guid locationId,
        UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new RequestValidationException(validationResult.ToErrors());

        Location? location = await _locationsRepository.GetByIdAsync(
            LocationId.Create(locationId),
            cancellationToken);

        if (location is null)
            throw new LocationNotFoundException(locationId);

        var name = LocationName.Create(request.Name);

        if (name != location.Name && await _locationsRepository.IsNameTakenAsync(name, cancellationToken))
            throw new LocationNameAlreadyTakenException(name.Value);

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
