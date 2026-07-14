namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Подразделение в ответах API.
/// </summary>
public sealed record DepartmentResponse(
    Guid Id,
    string Name,
    string Slug,
    string Path,
    Guid? ParentId);
