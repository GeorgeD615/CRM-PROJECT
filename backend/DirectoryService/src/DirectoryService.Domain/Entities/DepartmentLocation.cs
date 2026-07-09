using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

/// <summary>
/// Связь «многие-ко-многим» между подразделением и локацией. Хранит только id сторон,
/// без навигационных свойств. <see cref="IsPrimary"/> выделяет основную локацию
/// подразделения среди дополнительных.
/// </summary>
public sealed class DepartmentLocation
{
    private DepartmentLocation(
        DepartmentLocationId id,
        DepartmentId departmentId,
        LocationId locationId,
        bool isPrimary)
    {
        Id = id;
        DepartmentId = departmentId;
        LocationId = locationId;
        IsPrimary = isPrimary;
    }

    public DepartmentLocationId Id { get; }

    public DepartmentId DepartmentId { get; }

    public LocationId LocationId { get; }

    public bool IsPrimary { get; private set; }

    public static DepartmentLocation Create(
        DepartmentId departmentId,
        LocationId locationId,
        bool isPrimary)
    {
        ArgumentNullException.ThrowIfNull(departmentId);
        ArgumentNullException.ThrowIfNull(locationId);

        return new DepartmentLocation(
            DepartmentLocationId.Create(Guid.CreateVersion7()),
            departmentId,
            locationId,
            isPrimary);
    }

    public void MarkAsPrimary() => IsPrimary = true;

    public void UnmarkAsPrimary() => IsPrimary = false;
}
