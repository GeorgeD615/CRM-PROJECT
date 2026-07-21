using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: связи между подразделением и локацией не существует.
/// </summary>
public sealed class DepartmentLocationNotFoundException(Guid departmentId, Guid locationId) :
    NotFoundException([Error.NotFound($"Связь подразделения '{departmentId}' с локацией '{locationId}' не найдена.")]);
