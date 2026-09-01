using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations.CreateLocation;
using DirectoryService.Core.Locations.DeleteLocation;
using DirectoryService.Core.Locations.GetLocationById;
using DirectoryService.Core.Locations.GetLocations;
using DirectoryService.Core.Locations.GetTopLocations;
using DirectoryService.Core.Locations.UpdateLocation;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API локаций. Команды и сценарии чтения делегируются в Core,
/// контроллер только принимает параметры и отдаёт результат через <see cref="EndpointResult{TValue}"/>.
/// </summary>
[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] CreateLocationRequest request,
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> createLocationHandler,
        CancellationToken cancellationToken)
    {
        return EndpointResult<Guid>.Created(await createLocationHandler.HandleAsync(new(request), cancellationToken));
    }

    [HttpGet]
    [ProducesResponseType<Envelope<PagedResult<LocationListItemDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    public async Task<EndpointResult<PagedResult<LocationListItemDto>>> Get(
        [FromQuery] GetLocationsRequest request,
        [FromServices] IQueryHandler<PagedResult<LocationListItemDto>, GetLocationsQuery> getLocationsHandler,
        CancellationToken cancellationToken)
    {
        return await getLocationsHandler.HandleAsync(new(request), cancellationToken);
    }

    [HttpGet("top")]
    [ProducesResponseType<Envelope<IReadOnlyCollection<GetTopLocationDto>>>(StatusCodes.Status200OK)]
    public async Task<EndpointResult<IReadOnlyCollection<GetTopLocationDto>>> GetTop(
        [FromServices] IQueryHandler<IReadOnlyCollection<GetTopLocationDto>, GetTopLocationsQuery> getTopLocationsHandler,
        CancellationToken cancellationToken)
    {
        return await getTopLocationsHandler.HandleAsync(new(), cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Envelope<GetLocationDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public async Task<EndpointResult<GetLocationDto>> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetLocationDto, GetLocationByIdQuery> getLocationByIdHandler,
        CancellationToken cancellationToken)
    {
        return await getLocationByIdHandler.HandleAsync(new(id), cancellationToken);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationRequest request,
        [FromServices] ICommandHandler<UpdateLocationCommand> updateLocationHandler,
        CancellationToken cancellationToken)
    {
        return await updateLocationHandler.HandleAsync(new(id, request), cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteLocationCommand> deleteLocationHandler,
        CancellationToken cancellationToken)
    {
        return await deleteLocationHandler.HandleAsync(new(id), cancellationToken);
    }
}
