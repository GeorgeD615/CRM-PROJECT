namespace DirectoryService.Domain.ValueObjects;

public sealed record LocationAddress
{
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 150;
    public const int MaxHouseLength = 20;
    public const int MaxApartmentLength = 20;

    private LocationAddress(string city, string street, string house, string apartment)
    {
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }

    public string City { get; }

    public string Street { get; }

    public string House { get; }

    public string Apartment { get; }

    public static LocationAddress Create(string city, string street, string house, string apartment)
    {
        return new LocationAddress(
            Normalize(city, MaxCityLength, nameof(city)),
            Normalize(street, MaxStreetLength, nameof(street)),
            Normalize(house, MaxHouseLength, nameof(house)),
            Normalize(apartment, MaxApartmentLength, nameof(apartment)));
    }

    private static string Normalize(string value, int maxLength, string paramName)
    {
        if (value is null)
            throw new ArgumentNullException(paramName);

        string normalized = value.Trim();

        if (normalized.Length == 0)
            throw new ArgumentException($"Location address {paramName} must not be empty.", paramName);

        if (normalized.Length > maxLength)
            throw new ArgumentException(
                $"Location address {paramName} must not exceed {maxLength} characters.", paramName);

        return normalized;
    }
}
