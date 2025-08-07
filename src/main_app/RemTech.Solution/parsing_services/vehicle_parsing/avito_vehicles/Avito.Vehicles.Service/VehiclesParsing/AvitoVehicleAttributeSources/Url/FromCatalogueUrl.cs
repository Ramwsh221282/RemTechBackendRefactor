using Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Url;

public sealed class FromCatalogueUrl(CatalogueItem item) : IParsedVehicleUrlSource
{
    public Task<ParsedVehicleUrl> Read() => Task.FromResult(new ParsedVehicleUrl(item.ReadUrl()));
}
