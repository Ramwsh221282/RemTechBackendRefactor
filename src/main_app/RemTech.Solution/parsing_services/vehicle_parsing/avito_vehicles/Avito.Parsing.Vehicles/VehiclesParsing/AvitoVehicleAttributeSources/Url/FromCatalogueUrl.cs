using Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Url;

public sealed class FromCatalogueUrl(CatalogueItem item) : IParsedVehicleUrlSource
{
    public Task<ParsedVehicleUrl> Read() =>
        Task.FromResult(new ParsedVehicleUrl(item.ReadUrl()));
}