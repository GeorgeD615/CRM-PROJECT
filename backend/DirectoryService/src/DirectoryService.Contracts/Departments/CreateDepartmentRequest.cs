namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Запрос на создание подразделения. <paramref name="ParentId"/> равен null для корневого подразделения.
/// </summary>
public sealed record CreateDepartmentRequest(string Name, string Slug, Guid? ParentId);
