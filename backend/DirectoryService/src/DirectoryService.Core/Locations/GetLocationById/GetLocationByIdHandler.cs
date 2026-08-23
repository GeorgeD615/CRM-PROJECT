using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.GetLocationById;

/// <summary>
/// Сценарий чтения карточки локации: собирает DTO прямо в запросе, поэтому в БД уходит
/// SELECT по нужным колонкам, а доменная сущность не материализуется. Работает через
/// <see cref="IReadDbContext"/> — без tracking-а и без доступа к операциям записи.
/// </summary>
public sealed class GetLocationByIdHandler(
    IReadDbContext readDbContext,
    ILogger<GetLocationByIdHandler> logger) : IQueryHandler<GetLocationDto, GetLocationByIdQuery>
{

    public async Task<Result<GetLocationDto, Failure>> HandleAsync(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken)
    {
        var locationId = LocationId.Create(query.LocationId);

        GetLocationDto? location = await readDbContext.Locations
            .Where(l => l.Id == locationId)
            .Select(l => new GetLocationDto
            {
                Id = l.Id.Value,
                Name = l.Name.Value,
                City = l.Address.City,
                Street = l.Address.Street,
                House = l.Address.House,
                Apartment = l.Address.Apartment,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (location is null)
        {
            logger.LogWarning("Location {LocationId} not found.", query.LocationId);

            return Failure.From(Error.NotFound(
                $"Локация '{query.LocationId}' не найдена.",
                code: "directory.location.not_found"));
        }

        return location;
    }
}
