using CSharpFunctionalExtensions;
using DirectoryService.Shared;

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

    public static Result<LocationAddress, Failure> Create(string city, string street, string house, string apartment)
    {
        var errors = new List<Error>();

        string? normalizedCity = Validate(
            city, MaxCityLength, "Город обязателен.", $"Город не должен превышать {MaxCityLength} символов.", "location.address.city", errors);
        string? normalizedStreet = Validate(
            street, MaxStreetLength, "Улица обязательна.", $"Улица не должна превышать {MaxStreetLength} символов.", "location.address.street", errors);
        string? normalizedHouse = Validate(
            house, MaxHouseLength, "Дом обязателен.", $"Номер дома не должен превышать {MaxHouseLength} символов.", "location.address.house", errors);
        string? normalizedApartment = Validate(
            apartment, MaxApartmentLength, "Квартира обязательна.", $"Номер квартиры не должен превышать {MaxApartmentLength} символов.", "location.address.apartment", errors);

        if (errors.Count > 0)
            return new Failure(errors);

        // Проверки выше гарантируют, что все значения не null, когда список ошибок пуст.
        return new LocationAddress(normalizedCity!, normalizedStreet!, normalizedHouse!, normalizedApartment!);
    }

    private static string? Validate(
        string value,
        int maxLength,
        string requiredMessage,
        string tooLongMessage,
        string codePrefix,
        List<Error> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(Error.Validation(requiredMessage, code: $"{codePrefix}.required"));
            return null;
        }

        string normalized = value.Trim();

        if (normalized.Length > maxLength)
        {
            errors.Add(Error.Validation(tooLongMessage, code: $"{codePrefix}.too_long"));
            return null;
        }

        return normalized;
    }
}
