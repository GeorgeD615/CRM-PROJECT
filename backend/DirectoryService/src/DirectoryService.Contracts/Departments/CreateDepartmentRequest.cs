namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Запрос на создание подразделения. <paramref name="Name"/> — отображаемое название,
/// <paramref name="Slug"/> — стабильный технический сегмент для пути в дереве.
/// <paramref name="ParentId"/> равен null для корневого подразделения.
/// <paramref name="LocationIds"/> — локации, на которых работает подразделение; может быть пустым.
/// </summary>
public sealed record CreateDepartmentRequest(
    string Name,
    string Slug,
    Guid? ParentId,
    IReadOnlyList<Guid> LocationIds);
