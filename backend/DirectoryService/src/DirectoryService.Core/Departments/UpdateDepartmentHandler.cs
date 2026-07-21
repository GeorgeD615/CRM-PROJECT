using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments.Exceptions;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Extensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий обновления подразделения: меняет редактируемые поля существующего подразделения.
/// </summary>
public sealed class UpdateDepartmentHandler(
    IValidator<UpdateDepartmentRequest> validator,
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateDepartmentHandler> logger)
{
    private readonly IValidator<UpdateDepartmentRequest> _validator = validator;
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;
    private readonly ILogger<UpdateDepartmentHandler> _logger = logger;

    public async Task HandleAsync(
        Guid departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new RequestValidationException(validationResult.ToErrors());

        Department? department = await _departmentsRepository.GetByIdAsync(
            DepartmentId.Create(departmentId),
            cancellationToken);

        if (department is null)
            throw new DepartmentNotFoundException(departmentId);

        department.Rename(DepartmentName.Create(request.Name));

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Department {DepartmentId} updated.", departmentId);
    }
}
