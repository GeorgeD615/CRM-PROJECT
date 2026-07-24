using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Locations;
using DirectoryService.Web.Extensions;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocationRequest request,
        [FromServices] CreateLocationHandler createLocationHandler,
        CancellationToken cancellationToken)
    {
        var result = await createLocationHandler.HandleAsync(request, cancellationToken);

        return result.IsFailure
            ? result.Error.ToResponse()
            : CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
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

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationRequest request,
        [FromServices] UpdateLocationHandler updateLocationHandler,
        CancellationToken cancellationToken)
    {
        var result = await updateLocationHandler.HandleAsync(id, request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        return NoContent();
    }
}
