using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// Заглушка API подразделений: контракты и коды ответов настоящие, реализация придёт в следующих задачах.
/// </summary>
[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<DepartmentResponse>(StatusCodes.Status201Created)]
    public ActionResult<DepartmentResponse> Create([FromBody] CreateDepartmentRequest request)
    {
        var response = new DepartmentResponse(
            Guid.NewGuid(),
            request.Name,
            request.Slug,
            request.Slug,
            request.ParentId);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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
