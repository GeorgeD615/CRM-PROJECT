using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: подразделение с указанным id не существует.
/// </summary>
public sealed class DepartmentNotFoundException(Guid departmentId) :
    NotFoundException([Error.NotFound($"Подразделение '{departmentId}' не найдено.")]);