namespace DirectoryService.Contracts.Departments;

/// <summary>
/// Запрос на обновление подразделения. Slug неизменяем и потому в запросе отсутствует.
/// </summary>
public sealed record UpdateDepartmentRequest(string Name);
