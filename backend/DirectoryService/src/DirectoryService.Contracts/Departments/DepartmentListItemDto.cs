namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Строка таблицы подразделений: только то, что рисует список. Доменные типы,
/// дочерние подразделения и EF-навигации сюда не попадают.
/// </summary>
public sealed class DepartmentListItemDto
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Slug { get; init; }

    public required string Path { get; init; }

    public DateTime CreatedAt { get; init; }
}
