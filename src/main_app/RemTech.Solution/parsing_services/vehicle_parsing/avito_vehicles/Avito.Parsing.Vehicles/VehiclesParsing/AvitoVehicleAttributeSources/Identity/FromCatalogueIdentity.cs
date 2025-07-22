using Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class FromCatalogueIdentity(CatalogueItem item) : IParsedVehicleIdentitySource
{
    public Task<ParsedVehicleIdentity> Read() =>
        Task.FromResult(new ParsedVehicleIdentity(item.ReadId()));
}