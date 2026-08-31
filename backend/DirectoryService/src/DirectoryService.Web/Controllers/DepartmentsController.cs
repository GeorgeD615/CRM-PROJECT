using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Departments.AttachLocation;
using DirectoryService.Core.Departments.AttachPosition;
using DirectoryService.Core.Departments.CreateDepartment;
using DirectoryService.Core.Departments.DeleteDepartment;
using DirectoryService.Core.Departments.DetachLocation;
using DirectoryService.Core.Departments.DetachPosition;
using DirectoryService.Core.Departments.GetDepartmentById;
using DirectoryService.Core.Departments.GetDepartments;
using DirectoryService.Core.Departments.UpdateDepartment;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API подразделений. Команды и сценарии чтения делегируются в Core,
/// контроллер только принимает параметры и отдаёт результат через <see cref="EndpointResult{TValue}"/>.
/// </summary>
[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Envelope<Guid>>(StatusCodes.Status201Created)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] CreateDepartmentRequest request,
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> createDepartmentHandler,
        CancellationToken cancellationToken)
    {
        return EndpointResult<Guid>.Created(await createDepartmentHandler.HandleAsync(new(request), cancellationToken));
    }

    [HttpGet]
    [ProducesResponseType<Envelope<PagedResult<DepartmentListItemDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    public async Task<EndpointResult<PagedResult<DepartmentListItemDto>>> Get(
        [FromQuery] GetDepartmentsRequest request,
        [FromServices] IQueryHandler<PagedResult<DepartmentListItemDto>, GetDepartmentsQuery> getDepartmentsHandler,
        CancellationToken cancellationToken)
    {
        return await getDepartmentsHandler.HandleAsync(new(request), cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Envelope<GetDepartmentDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public async Task<EndpointResult<GetDepartmentDto>> GetById(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetDepartmentDto, GetDepartmentByIdQuery> getDepartmentByIdHandler,
        CancellationToken cancellationToken)
    {
        return await getDepartmentByIdHandler.HandleAsync(new(id), cancellationToken);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public async Task<EndpointResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        [FromServices] ICommandHandler<UpdateDepartmentCommand> updateDepartmentHandler,
        CancellationToken cancellationToken)
    {
        return await updateDepartmentHandler.HandleAsync(new(id, request), cancellationToken);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status201Created)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> AttachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] ICommandHandler<AttachLocationCommand> attachLocationHandler,
        CancellationToken cancellationToken)
    {
        return EndpointResult.Created(await attachLocationHandler.HandleAsync(new(departmentId, locationId), cancellationToken));
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public async Task<EndpointResult> DetachLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        [FromServices] ICommandHandler<DetachLocationCommand> detachLocationHandler,
        CancellationToken cancellationToken)
    {
        return await detachLocationHandler.HandleAsync(new(departmentId, locationId), cancellationToken);
    }

    [HttpPost("{departmentId:guid}/positions/{positionId:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status201Created)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> AttachPosition(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        [FromServices] ICommandHandler<AttachPositionCommand> attachPositionHandler,
        CancellationToken cancellationToken)
    {
        return EndpointResult.Created(await attachPositionHandler.HandleAsync(new(departmentId, positionId), cancellationToken));
    }

    [HttpDelete("{departmentId:guid}/positions/{positionId:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public async Task<EndpointResult> DetachPosition(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        [FromServices] ICommandHandler<DetachPositionCommand> detachPositionHandler,
        CancellationToken cancellationToken)
    {
        return await detachPositionHandler.HandleAsync(new(departmentId, positionId), cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Envelope>(StatusCodes.Status409Conflict)]
    public async Task<EndpointResult> Delete(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteDepartmentCommand> deleteDepartmentHandler,
        CancellationToken cancellationToken)
    {
        return await deleteDepartmentHandler.HandleAsync(new(id), cancellationToken);
    }
}
