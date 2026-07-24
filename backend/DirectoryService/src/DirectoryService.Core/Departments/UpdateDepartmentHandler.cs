using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий обновления подразделения: меняет редактируемые поля существующего подразделения.
/// Не бросает и не логирует — все ошибки возвращаются как результат.
/// </summary>
public sealed class UpdateDepartmentHandler(
    IValidator<UpdateDepartmentRequest> validator,
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager)
{
    private readonly IValidator<UpdateDepartmentRequest> _validator = validator;
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<UnitResult<Failure>> HandleAsync(
        Guid departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToErrors();

        Result<Department, Failure> departmentResult = await _departmentsRepository.GetByIdAsync(
            DepartmentId.Create(departmentId),
            cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        Department department = departmentResult.Value;

        UnitResult<Failure> renameResult = department.Rename(DepartmentName.Create(request.Name));
        if (renameResult.IsFailure)
            return renameResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Failure>();
    }
}
