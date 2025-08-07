using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehiclePrices;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithPrice(IParsedVehiclePriceSource source, IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehiclePrice price = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, price);
    }
}
