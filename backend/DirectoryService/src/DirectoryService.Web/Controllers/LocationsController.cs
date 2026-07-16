using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API локаций. Создание делегируется в Core, остальные операции — заглушки до следующих задач.
/// </summary>
[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateLocationRequest request,
        [FromServices] CreateLocationHandler createLocationHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid id = await createLocationHandler.HandleAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
        catch (LocationNameAlreadyTakenException exception)
        {
            return Conflict(exception.Message);
        }
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
