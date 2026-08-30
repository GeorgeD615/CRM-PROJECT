using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.GetTopLocations;

/// <summary>
/// Топ локаций по числу привязанных подразделений. Размер выборки фиксирован:
/// параметр limit появится, когда UI действительно начнёт его задавать.
/// </summary>
public record GetTopLocationsQuery : IQuery
{
    public const int TopSize = 5;
}
