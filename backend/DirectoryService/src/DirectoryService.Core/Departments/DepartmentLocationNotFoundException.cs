namespace DirectoryService.Core.Departments;

/// <summary>
/// Нарушение бизнес-правила: связи между подразделением и локацией не существует.
/// </summary>
public sealed class DepartmentLocationNotFoundException : Exception
{
    public DepartmentLocationNotFoundException(Guid departmentId, Guid locationId)
        : base($"Связь подразделения '{departmentId}' с локацией '{locationId}' не найдена.")
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid DepartmentId { get; }

    public Guid LocationId { get; }
}
