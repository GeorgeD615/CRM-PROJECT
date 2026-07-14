namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Локация в ответах API.
/// </summary>
public sealed record LocationResponse(
    Guid Id,
    string Name,
    AddressDto Address);
