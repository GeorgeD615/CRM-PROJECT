using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.DeleteLocation;

/// <summary>
/// Сценарий удаления локации: запрещает удаление, пока локация привязана к подразделениям
/// (её нельзя удалить, пока на неё ссылаются). Не бросает; commit — через TransactionManager.
/// </summary>
public sealed class DeleteLocationHandler(
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteLocationHandler> logger) : ICommandHandler<DeleteLocationCommand>
{
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<DeleteLocationHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(DeleteLocationCommand command, CancellationToken cancellationToken)
    {
        var locationId = LocationId.Create(command.LocationId);

        Result<Location, Failure> locationResult = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        bool hasLinks = await _locationsRepository.HasLinkedDepartmentsAsync(locationId, cancellationToken);
        if (hasLinks)
        {
            _logger.LogWarning("Location {LocationId} cannot be deleted while attached to departments.", command.LocationId);

            return Failure.From(Error.Conflict(
                "Нельзя удалить локацию, пока она привязана к подразделениям.",
                code: "directory.location.has_links"));
        }

        _locationsRepository.Remove(locationResult.Value);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Location {LocationId} deleted.", command.LocationId);

        return UnitResult.Success<Failure>();
    }
}
