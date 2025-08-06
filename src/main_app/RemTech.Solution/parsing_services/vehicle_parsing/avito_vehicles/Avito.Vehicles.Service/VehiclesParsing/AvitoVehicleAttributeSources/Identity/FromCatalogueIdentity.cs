using Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class FromCatalogueIdentity(CatalogueItem item) : IParsedVehicleIdentitySource
{
    public Task<ParsedVehicleIdentity> Read() =>
        Task.FromResult(new ParsedVehicleIdentity(item.ReadId()));
}
