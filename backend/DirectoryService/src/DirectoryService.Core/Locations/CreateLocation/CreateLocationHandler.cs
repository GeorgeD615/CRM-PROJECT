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

namespace DirectoryService.Core.Locations.CreateLocation;

/// <summary>
/// Сценарий создания локации: валидирует запрос, проверяет уникальность имени
/// и возвращает id созданной локации либо <see cref="Failure"/>. Не бросает; успех логируется
/// как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class CreateLocationHandler(
    IValidator<CreateLocationRequest> validator,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<CreateLocationHandler> logger) : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly IValidator<CreateLocationRequest> _validator = validator;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<CreateLocationHandler> _logger = logger;

    public async Task<Result<Guid, Failure>> HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        CreateLocationRequest request = command.CreateLocationDto;

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        var name = LocationName.Create(request.Name).Value;
        var address = LocationAddress.Create(
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment).Value;

        bool isNameTaken = await _locationsRepository.IsNameTakenAsync(name, cancellationToken);
        if (isNameTaken)
        {
            _logger.LogWarning("Location name {LocationName} is already taken.", name.Value);

            return Failure.From(Error.Conflict($"Локация с именем '{name.Value}' уже существует.", code: "directory.location.name_conflict"));
        }

        Result<Location, Failure> locationResult = Location.Create(name, address);
        if (locationResult.IsFailure)
            return locationResult.Error;

        Location location = locationResult.Value;

        _locationsRepository.Add(location);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Location {LocationId} created with name {LocationName}.", location.Id.Value, location.Name.Value);

        return location.Id.Value;
    }
}
