using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.DeleteDepartment;

/// <summary>
/// Сценарий удаления подразделения: запрещает удаление, пока у него есть дочерние подразделения;
/// собственные связи с локациями и должностями удаляются каскадно на уровне БД в той же транзакции.
/// Не бросает; commit — через TransactionManager.
/// </summary>
public sealed class DeleteDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteDepartmentHandler> logger) : ICommandHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<DeleteDepartmentHandler> _logger = logger;

    public async Task<UnitResult<Failure>> HandleAsync(DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(command.DepartmentId);

        Result<Department, Failure> departmentResult = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        bool hasChildren = await _departmentsRepository.HasChildrenAsync(departmentId, cancellationToken);
        if (hasChildren)
        {
            _logger.LogWarning("Department {DepartmentId} cannot be deleted while it has child departments.", command.DepartmentId);

            return Failure.From(Error.Conflict(
                "Нельзя удалить подразделение, пока у него есть дочерние подразделения.",
                code: "directory.department.has_children"));
        }

        _departmentsRepository.Remove(departmentResult.Value);

        UnitResult<Failure> saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        _logger.LogInformation("Department {DepartmentId} deleted with its location and position links.", command.DepartmentId);

        return UnitResult.Success<Failure>();
    }
}
