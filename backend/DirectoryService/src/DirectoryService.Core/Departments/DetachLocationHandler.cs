using CSharpFunctionalExtensions;
using DirectoryService.Core.Database;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments;

/// <summary>
/// Сценарий отвязки локации от подразделения: удаляет существующую связь.
/// Не бросает и не логирует — все ошибки возвращаются как результат.
/// </summary>
public sealed class DetachLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager)
{
    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ITransactionManager _transactionManager = transactionManager;

    public async Task<UnitResult<Failure>> HandleAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        Result<DepartmentLocation, Failure> linkResult = await _departmentsRepository.GetDepartmentLocationAsync(
            DepartmentId.Create(departmentId),
            LocationId.Create(locationId),
            cancellationToken);
        if (linkResult.IsFailure)
            return linkResult.Error;

        UnitResult<Failure> removeResult = _departmentsRepository.RemoveDepartmentLocation(linkResult.Value);
        if (removeResult.IsFailure)
            return removeResult.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        return UnitResult.Success<Failure>();
    }
}
