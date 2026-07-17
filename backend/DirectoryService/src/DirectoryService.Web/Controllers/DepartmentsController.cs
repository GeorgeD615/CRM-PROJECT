using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API подразделений. Создание делегируется в Core, остальные операции — заглушки до следующих задач.
/// </summary>
[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] CreateDepartmentHandler createDepartmentHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            Guid id = await createDepartmentHandler.HandleAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
        catch (ParentDepartmentNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (LocationsNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<DepartmentResponse>>(StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<DepartmentResponse>> GetAll()
    {
        return Ok(Array.Empty<DepartmentResponse>());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DepartmentResponse> GetById([FromRoute] Guid id)
    {
        return NotFound();
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        [FromServices] UpdateDepartmentHandler updateDepartmentHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            await updateDepartmentHandler.HandleAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (DepartmentNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AttachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] AttachLocationHandler attachLocationHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            await attachLocationHandler.HandleAsync(departmentId, locationId, cancellationToken);
            return Created();
        }
        catch (DepartmentNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (LocationNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (DepartmentLocationAlreadyExistsException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DetachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] DetachLocationHandler detachLocationHandler,
        CancellationToken cancellationToken)
    {
        try
        {
            await detachLocationHandler.HandleAsync(departmentId, locationId, cancellationToken);
            return NoContent();
        }
        catch (DepartmentLocationNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        return NoContent();
    }
}
