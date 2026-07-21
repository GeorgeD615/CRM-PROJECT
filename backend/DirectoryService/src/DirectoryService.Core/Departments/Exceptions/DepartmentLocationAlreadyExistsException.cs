using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: локация уже привязана к подразделению.
/// </summary>
public sealed class DepartmentLocationAlreadyExistsException(Guid departmentId, Guid locationId) :
    ConflictException([Error.Conflict($"Локация '{locationId}' уже привязана к подразделению '{departmentId}'.")]);
