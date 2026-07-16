namespace DirectoryService.Core.Departments;

/// <summary>
/// Нарушение бизнес-правила: указанное родительское подразделение не существует.
/// Создание дочернего подразделения без родителя невозможно — операция отклоняется целиком.
/// </summary>
public sealed class ParentDepartmentNotFoundException : Exception
{
    public ParentDepartmentNotFoundException(Guid parentId)
        : base($"Родительское подразделение '{parentId}' не найдено.")
    {
        ParentId = parentId;
    }

    public Guid ParentId { get; }
}
