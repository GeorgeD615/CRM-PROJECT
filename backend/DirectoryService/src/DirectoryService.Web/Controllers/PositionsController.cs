using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Positions.CreatePosition;
using DirectoryService.Core.Positions.DeletePosition;
using DirectoryService.Core.Positions.UpdatePosition;
using DirectoryService.Shared;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API должностей. Запись делегируется в Core, чтение — заглушки до следующих задач.
/// </summary>
[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] CreatePositionRequest request,
        [FromServices] ICommandHandler<Guid, CreatePositionCommand> createPositionHandler,
        CancellationToken cancellationToken)
    {
        return EndpointResult<Guid>.Created(await createPositionHandler.HandleAsync(new(request), cancellationToken));
    }

    [HttpGet]
    [ProducesResponseType<Envelope<IReadOnlyCollection<PositionResponse>>>(StatusCodes.Status200OK)]
    public EndpointResult<IReadOnlyCollection<PositionResponse>> GetAll()
    {
        // Заглушка: чтение ещё не реализовано (нет query-хэндлера).
        IReadOnlyCollection<PositionResponse> positions = [];
        return Result.Success<IReadOnlyCollection<PositionResponse>, Failure>(positions);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Envelope<PositionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public EndpointResult<PositionResponse> GetById([FromRoute] Guid id)
    {
        // Заглушка: чтение ещё не реализовано (нет query-хэндлера).
        return Result.Failure<PositionResponse, Failure>(
            Error.NotFound($"Должность '{id}' не найдена.", code: "directory.position.not_found"));
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePositionRequest request,
        [FromServices] ICommandHandler<UpdatePositionCommand> updatePositionHandler,
        CancellationToken cancellationToken)
    {
        return await updatePositionHandler.HandleAsync(new(id, request), cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeletePositionCommand> deletePositionHandler,
        CancellationToken cancellationToken)
    {
        return await deletePositionHandler.HandleAsync(new(id), cancellationToken);
    }
}
