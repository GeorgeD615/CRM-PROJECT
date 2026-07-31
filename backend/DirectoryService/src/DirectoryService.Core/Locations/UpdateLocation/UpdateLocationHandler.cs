using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Validations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.UpdateLocation;

/// <summary>
/// Сценарий обновления локации: меняет имя и адрес существующей локации,
/// не допуская дубля имени с другой локацией. Не бросает; успех логируется
/// как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class UpdateLocationHandler(
    IValidator<UpdateLocationRequest> validator,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateLocationHandler> logger) : ICommandHandler<UpdateLocationCommand>
{
    private readonly IValidator<UpdateLocationRequest> _validator = validator;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<UpdateLocationHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        Guid locationId = command.LocationId;
        UpdateLocationRequest request = command.UpdateLocationDto;

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        Result<Location, Failure> locationResult = await _locationsRepository.GetByIdAsync(
            LocationId.Create(locationId),
            cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        Location location = locationResult.Value;

        var name = LocationName.Create(request.Name).Value;

        if (name != location.Name)
        {
            bool isNameTaken = await _locationsRepository.IsNameTakenAsync(name, cancellationToken);
            if (isNameTaken)
            {
                _logger.LogWarning("Location name {LocationName} is already taken.", name.Value);

                return Failure.From(Error.Conflict($"Локация с именем '{name.Value}' уже существует.", code: "directory.location.name_conflict"));
            }
        }

        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment).Value;

        UnitResult<Failure> renameResult = location.Rename(name);
        if (renameResult.IsFailure)
            return renameResult.Error;

        UnitResult<Failure> changeAddressResult = location.ChangeAddress(address);
        if (changeAddressResult.IsFailure)
            return changeAddressResult.Error;

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Location {LocationId} updated.", locationId);

        return UnitResult.Success<Failure>();
    }
}
