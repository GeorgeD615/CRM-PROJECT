namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Карточка подразделения для UI: плоский набор полей без доменных типов
/// и без коллекций связанных сущностей.
/// </summary>
public sealed class GetDepartmentDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Slug { get; init; }

    public required string Path { get; init; }

    public Guid? ParentId { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
