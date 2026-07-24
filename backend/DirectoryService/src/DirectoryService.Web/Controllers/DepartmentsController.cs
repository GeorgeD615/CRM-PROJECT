using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
using DirectoryService.Web.Extensions;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] CreateDepartmentHandler createDepartmentHandler,
        CancellationToken cancellationToken)
    {
        var result = await createDepartmentHandler.HandleAsync(request, cancellationToken);

        return result.IsFailure
            ? result.Error.ToResponse()
            : CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        [FromServices] UpdateDepartmentHandler updateDepartmentHandler,
        CancellationToken cancellationToken)
    {
        var result = await updateDepartmentHandler.HandleAsync(id, request, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : NoContent();
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
        var result = await attachLocationHandler.HandleAsync(departmentId, locationId, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Created();
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
        var result = await detachLocationHandler.HandleAsync(departmentId, locationId, cancellationToken);

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
