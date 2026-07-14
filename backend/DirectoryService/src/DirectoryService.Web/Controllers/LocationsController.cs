using DirectoryService.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// Заглушка API локаций: контракты и коды ответов настоящие, реализация придёт в следующих задачах.
/// </summary>
[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<LocationResponse>(StatusCodes.Status201Created)]
    public ActionResult<LocationResponse> Create([FromBody] CreateLocationRequest request)
    {
        var response = new LocationResponse(Guid.NewGuid(), request.Name, request.Address);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<LocationResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<LocationResponse>> GetAll()
    {
        return Ok(Array.Empty<LocationResponse>());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<LocationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<LocationResponse> GetById([FromRoute] Guid id)
    {
        return NotFound();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<LocationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<LocationResponse> Update([FromRoute] Guid id, [FromBody] UpdateLocationRequest request)
    {
        var response = new LocationResponse(id, request.Name, request.Address);

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
