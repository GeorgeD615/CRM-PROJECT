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
/// Сценарий создания локации: валидирует запрос, проверяет уникальность имени
/// и возвращает id созданной локации либо <see cref="Failure"/>. Не бросает и не логирует —
/// все ошибки возвращаются как результат.
/// </summary>
public sealed class CreateLocationHandler(
    IValidator<CreateLocationRequest> validator,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager)
{
    private readonly IValidator<CreateLocationRequest> _validator = validator;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<Result<Guid, Failure>> HandleAsync(CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var name = LocationName.Create(request.Name);
        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment);

        Result<bool, Failure> isNameTakenResult = await _locationsRepository.IsNameTakenAsync(name, cancellationToken);
        if (isNameTakenResult.IsFailure)
            return isNameTakenResult.Error;

        if (isNameTakenResult.Value)
            return Failure.From(Error.Conflict($"Локация с именем '{name.Value}' уже существует.", code: "directory.location.name_conflict"));

        Result<Location, Failure> locationResult = Location.Create(name, address);
        if (locationResult.IsFailure)
            return locationResult.Error;

        Location location = locationResult.Value;

        UnitResult<Failure> addResult = _locationsRepository.Add(location);
        if (addResult.IsFailure)
            return addResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return location.Id.Value;
    }
}
