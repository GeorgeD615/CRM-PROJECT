namespace DirectoryService.Contracts.Positions;

/// <summary>
/// Должность в ответах API.
/// </summary>
public sealed record PositionResponse(
    Guid Id,
    string Name);
