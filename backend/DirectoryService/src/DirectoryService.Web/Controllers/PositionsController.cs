using DirectoryService.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// Заглушка API должностей: контракты и коды ответов настоящие, реализация придёт в следующих задачах.
/// </summary>
[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PositionResponse>(StatusCodes.Status201Created)]
    public ActionResult<PositionResponse> Create([FromBody] CreatePositionRequest request)
    {
        var response = new PositionResponse(Guid.NewGuid(), request.Name);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<PositionResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<PositionResponse>> GetAll()
    {
        return Ok(Array.Empty<PositionResponse>());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<PositionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PositionResponse> GetById([FromRoute] Guid id)
    {
        return NotFound();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<PositionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PositionResponse> Update([FromRoute] Guid id, [FromBody] UpdatePositionRequest request)
    {
        var response = new PositionResponse(id, request.Name);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        return NoContent();
    }
}
