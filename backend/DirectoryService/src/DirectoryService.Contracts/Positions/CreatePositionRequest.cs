namespace DirectoryService.Contracts.Positions;

/// <summary>
/// Запрос на создание должности.
/// </summary>
public sealed record CreatePositionRequest(string Name);
