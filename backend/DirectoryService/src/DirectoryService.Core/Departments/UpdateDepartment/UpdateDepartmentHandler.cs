using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.UpdateDepartment;

/// <summary>
/// Сценарий обновления подразделения: меняет редактируемые поля существующего подразделения.
/// Не бросает; успех логируется как бизнес-событие, ожидаемые отказы возвращаются как результат.
/// </summary>
public sealed class UpdateDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateDepartmentHandler> logger) : ICommandHandler<UpdateDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<UpdateDepartmentHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        Guid departmentId = command.DepartmentId;
        UpdateDepartmentRequest request = command.UpdateDepartmentDto;

        Result<Department, Failure> departmentResult = await _departmentsRepository.GetByIdAsync(
            DepartmentId.Create(departmentId),
            cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        Department department = departmentResult.Value;

        var name = DepartmentName.Create(request.Name).Value;

        UnitResult<Failure> renameResult = department.Rename(name);
        if (renameResult.IsFailure)
            return renameResult.Error;

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Department {DepartmentId} renamed to {DepartmentName}.", departmentId, name.Value);

        return UnitResult.Success<Failure>();
    }
}
