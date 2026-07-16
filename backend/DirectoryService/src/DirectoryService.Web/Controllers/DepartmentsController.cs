using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DepartmentResponse> Update([FromRoute] Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        var response = new DepartmentResponse(
            id,
            request.Name,
            "stub-slug",
            "stub-slug",
            null);

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
