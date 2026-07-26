using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Shared;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// Заглушка API должностей: контракты, коды ответов и Envelope-форма настоящие,
/// доменная реализация (Core) придёт в следующих задачах.
/// </summary>
[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<PositionResponse>>(StatusCodes.Status201Created)]
    public EndpointResult<PositionResponse> Create([FromBody] CreatePositionRequest request)
    {
        // Заглушка: возвращает переданные данные как созданный ресурс.
        var response = new PositionResponse(Guid.NewGuid(), request.Name);
        return EndpointResult<PositionResponse>.Created(Result.Success<PositionResponse, Failure>(response));
    }

    [HttpGet]
    [ProducesResponseType<Envelope<IReadOnlyCollection<PositionResponse>>>(StatusCodes.Status200OK)]
    public EndpointResult<IReadOnlyCollection<PositionResponse>> GetAll()
    {
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType<Envelope<PositionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public EndpointResult<PositionResponse> Update([FromRoute] Guid id, [FromBody] UpdatePositionRequest request)
    {
        // Заглушка: возвращает переданные данные как обновлённый ресурс.
        var response = new PositionResponse(id, request.Name);
        return Result.Success<PositionResponse, Failure>(response);
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
