using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

/// <summary>
/// Связь «многие-ко-многим» между подразделением и должностью. Хранит только id сторон,
/// без навигационных свойств.
/// </summary>
public sealed class DepartmentPosition
{
    private DepartmentPosition(
        DepartmentPositionId id,
        DepartmentId departmentId,
        PositionId positionId)
    {
        Id = id;
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public DepartmentPositionId Id { get; }

    public DepartmentId DepartmentId { get; }

    public PositionId PositionId { get; }

    public static DepartmentPosition Create(DepartmentId departmentId, PositionId positionId)
    {
        ArgumentNullException.ThrowIfNull(departmentId);
        ArgumentNullException.ThrowIfNull(positionId);

        return new DepartmentPosition(
            DepartmentPositionId.Create(Guid.CreateVersion7()),
            departmentId,
            positionId);
    }
}
