using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий обновления подразделения: меняет редактируемые поля существующего подразделения.
/// </summary>
public sealed class UpdateDepartmentHandler
{
    private readonly IValidator<UpdateDepartmentRequest> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<UpdateDepartmentHandler> _logger;

    public UpdateDepartmentHandler(
        IValidator<UpdateDepartmentRequest> validator,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        ILogger<UpdateDepartmentHandler> logger)
    {
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task HandleAsync(
        Guid departmentId,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        Department? department = await _departmentsRepository.GetByIdAsync(
            DepartmentId.Create(departmentId),
            cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Department {DepartmentId} does not exist.", departmentId);

            throw new DepartmentNotFoundException(departmentId);
        }

        department.Rename(DepartmentName.Create(request.Name));

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Department {DepartmentId} updated.", departmentId);
    }
}
