namespace DirectoryService.Core.Departments;

/// <summary>
/// Нарушение бизнес-правила: подразделение с указанным id не существует.
/// </summary>
public sealed class DepartmentNotFoundException : Exception
{
    public DepartmentNotFoundException(Guid departmentId)
        : base($"Подразделение '{departmentId}' не найдено.")
    {
        DepartmentId = departmentId;
    }

    public Guid DepartmentId { get; }
}
