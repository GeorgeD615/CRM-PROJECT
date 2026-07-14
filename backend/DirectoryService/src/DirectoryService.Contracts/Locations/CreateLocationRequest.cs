namespace DirectoryService.Contracts.Locations;

/// <summary>
/// Запрос на создание локации.
/// </summary>
public sealed record CreateLocationRequest(string Name, AddressDto Address);
