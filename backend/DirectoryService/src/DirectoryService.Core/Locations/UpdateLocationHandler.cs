using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Locations;

/// <summary>
/// Сценарий обновления локации: меняет имя и адрес существующей локации,
/// не допуская дубля имени с другой локацией. Не бросает и не логирует —
/// все ошибки возвращаются как результат.
/// </summary>
public sealed class UpdateLocationHandler(
    IValidator<UpdateLocationRequest> validator,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager)
{
    private readonly IValidator<UpdateLocationRequest> _validator = validator;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<UnitResult<Failure>> HandleAsync(
        Guid locationId,
        UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        Result<Location, Failure> locationResult = await _locationsRepository.GetByIdAsync(
            LocationId.Create(locationId),
            cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        Location location = locationResult.Value;

        var name = LocationName.Create(request.Name);

        if (name != location.Name)
        {
            Result<bool, Failure> isNameTakenResult = await _locationsRepository.IsNameTakenAsync(name, cancellationToken);
            if (isNameTakenResult.IsFailure)
                return isNameTakenResult.Error;

            if (isNameTakenResult.Value)
                return Failure.From(Error.Conflict($"Локация с именем '{name.Value}' уже существует.", code: "directory.location.name_conflict"));
        }

        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment);

        UnitResult<Failure> renameResult = location.Rename(name);
        if (renameResult.IsFailure)
            return renameResult.Error;

        UnitResult<Failure> changeAddressResult = location.ChangeAddress(address);
        if (changeAddressResult.IsFailure)
            return changeAddressResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Failure>();
    }
}
