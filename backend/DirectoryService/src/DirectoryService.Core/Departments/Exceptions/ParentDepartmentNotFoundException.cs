using DirectoryService.Core.Exceptions;
using DirectoryService.Shared;

namespace DirectoryService.Core.Departments.Exceptions;

/// <summary>
/// Нарушение бизнес-правила: указанное родительское подразделение не существует.
/// Создание дочернего подразделения без родителя невозможно — операция отклоняется целиком.
/// </summary>
public sealed class ParentDepartmentNotFoundException(Guid parentId) :
    NotFoundException([Error.NotFound(
        $"Родительское подразделение '{parentId}' не найдено.",
        code: "directory.department.parent_not_found")]);
