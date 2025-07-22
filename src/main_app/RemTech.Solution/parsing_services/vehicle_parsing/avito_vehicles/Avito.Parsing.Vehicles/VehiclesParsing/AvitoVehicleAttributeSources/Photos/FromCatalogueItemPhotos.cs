using Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePhotos;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Photos;

public sealed class FromCatalogueItemPhotos(CatalogueItem item) : IParsedVehiclePhotos
{
    public Task<UniqueParsedVehiclePhotos> Read() =>
        Task.FromResult(new UniqueParsedVehiclePhotos(item.ReadPhotos().Select(p => (string)p)));
}