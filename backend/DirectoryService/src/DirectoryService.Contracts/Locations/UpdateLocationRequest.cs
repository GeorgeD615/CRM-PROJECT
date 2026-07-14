namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Запрос на обновление локации.
/// </summary>
public sealed record UpdateLocationRequest(string Name, AddressDto Address);
