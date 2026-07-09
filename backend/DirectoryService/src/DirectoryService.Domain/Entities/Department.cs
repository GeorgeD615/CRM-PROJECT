using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

/// <summary>
/// Подразделение компании. Образует иерархию через <see cref="ParentId"/> и хранит
/// стабильный путь <see cref="Path"/> в дереве оргструктуры, собранный из slug'ов
/// родителей и собственного <see cref="Slug"/>.
/// </summary>
public sealed class Department
{
    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentSlug slug,
        DepartmentPath path,
        DepartmentId? parentId)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Path = path;
        ParentId = parentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public DepartmentId Id { get; }

    public DepartmentName Name { get; private set; }

    public DepartmentSlug Slug { get; }

    public DepartmentPath Path { get; private set; }

    public DepartmentId? ParentId { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public bool IsRoot => ParentId is null;

    public static Department CreateRoot(DepartmentName name, DepartmentSlug slug)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(slug);

        return new Department(
            DepartmentId.Create(Guid.CreateVersion7()),
            name,
            slug,
            DepartmentPath.CreateRoot(slug),
            parentId: null);
    }

    public static Department CreateChild(DepartmentName name, DepartmentSlug slug, Department parent)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(parent);

        return new Department(
            DepartmentId.Create(Guid.CreateVersion7()),
            name,
            slug,
            parent.Path.CreateChild(slug),
            parent.Id);
    }

    public void Rename(DepartmentName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}
