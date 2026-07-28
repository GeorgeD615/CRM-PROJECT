using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Departments.AttachLocation;
using DirectoryService.Core.Departments.CreateDepartment;
using DirectoryService.Core.Departments.DetachLocation;
using DirectoryService.Core.Departments.UpdateDepartment;
using DirectoryService.Shared;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

/// <summary>
/// API подразделений. Создание, обновление и связи с локациями делегируются в Core,
/// чтение — заглушки до следующих задач.
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
    [ProducesResponseType<Envelope<IReadOnlyCollection<DepartmentResponse>>>(StatusCodes.Status200OK)]
    public EndpointResult<IReadOnlyCollection<DepartmentResponse>> GetAll()
    {
        // Заглушка: чтение ещё не реализовано (нет query-хэндлера).
        IReadOnlyCollection<DepartmentResponse> departments = [];
        return Result.Success<IReadOnlyCollection<DepartmentResponse>, Failure>(departments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<Envelope<DepartmentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public EndpointResult<DepartmentResponse> GetById([FromRoute] Guid id)
    {
        // Заглушка: чтение ещё не реализовано (нет query-хэндлера).
        return Result.Failure<DepartmentResponse, Failure>(
            Error.NotFound($"Подразделение '{id}' не найдено.", code: "directory.department.not_found"));
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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<Envelope>(StatusCodes.Status200OK)]
    [ProducesResponseType<Envelope>(StatusCodes.Status404NotFound)]
    public EndpointResult Delete([FromRoute] Guid id)
    {
        // Заглушка: удаление ещё не реализовано (нет команды).
        return UnitResult.Success<Failure>();
    }
}
