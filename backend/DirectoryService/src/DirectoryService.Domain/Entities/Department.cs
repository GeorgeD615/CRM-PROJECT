using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Shared;

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

    public static Result<Department, Failure> CreateRoot(DepartmentName name, DepartmentSlug slug)
    {
        var errors = new List<Error>();

        if (name is null)
            errors.Add(Error.Validation("Имя подразделения обязательно.", nameof(name)));

        if (slug is null)
            errors.Add(Error.Validation("Slug подразделения обязателен.", nameof(slug)));

        if (errors.Count > 0)
            return new Failure(errors);

        // Проверки выше гарантируют, что оба значения не null, когда список ошибок пуст.
        return new Department(
            DepartmentId.Create(Guid.CreateVersion7()),
            name!,
            slug!,
            DepartmentPath.CreateRoot(slug!),
            parentId: null);
    }

    public static Result<Department, Failure> CreateChild(DepartmentName name, DepartmentSlug slug, Department parent)
    {
        var errors = new List<Error>();

        if (name is null)
            errors.Add(Error.Validation("Имя подразделения обязательно.", nameof(name)));

        if (slug is null)
            errors.Add(Error.Validation("Slug подразделения обязателен.", nameof(slug)));

        if (parent is null)
            errors.Add(Error.Validation("Родительское подразделение обязательно.", nameof(parent)));

        if (errors.Count > 0)
            return new Failure(errors);

        // Проверки выше гарантируют, что значения не null, когда список ошибок пуст.
        return new Department(
            DepartmentId.Create(Guid.CreateVersion7()),
            name!,
            slug!,
            parent!.Path.CreateChild(slug!),
            parent.Id);
    }

    public UnitResult<Failure> Rename(DepartmentName name)
    {
        if (name is null)
            return Failure.From(Error.Validation("Имя подразделения обязательно.", nameof(name)));

        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }
}
