namespace DirectoryService.Core.Departments;

/// <summary>
/// Нарушение бизнес-правила: локация уже привязана к подразделению.
/// </summary>
public sealed class DepartmentLocationAlreadyExistsException : Exception
{
    public DepartmentLocationAlreadyExistsException(Guid departmentId, Guid locationId)
        : base($"Локация '{locationId}' уже привязана к подразделению '{departmentId}'.")
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    public Guid DepartmentId { get; }

    public Guid LocationId { get; }
}
