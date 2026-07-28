using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations.CreateLocation;
using DirectoryService.Core.Locations.UpdateLocation;
using DirectoryService.Shared;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API локаций. Создание и обновление делегируются в Core, остальные операции — заглушки до следующих задач.
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
    [ProducesResponseType<Envelope<IReadOnlyCollection<LocationResponse>>>(StatusCodes.Status200OK)]
    public EndpointResult<IReadOnlyCollection<LocationResponse>> GetAll()
    {
        // Заглушка: чтение ещё не реализовано (нет query-хэндлера).
        IReadOnlyCollection<LocationResponse> locations = [];
        return Result.Success<IReadOnlyCollection<LocationResponse>, Failure>(locations);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Envelope<LocationResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public EndpointResult<LocationResponse> GetById([FromRoute] Guid id)
    {
        // Заглушка: чтение ещё не реализовано (нет query-хэндлера).
        return Result.Failure<LocationResponse, Failure>(
            Error.NotFound($"Локация '{id}' не найдена.", code: "directory.location.not_found"));
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
    public EndpointResult Delete([FromRoute] Guid id)
    {
        // Заглушка: удаление ещё не реализовано (нет команды).
        return UnitResult.Success<Failure>();
    }
}
