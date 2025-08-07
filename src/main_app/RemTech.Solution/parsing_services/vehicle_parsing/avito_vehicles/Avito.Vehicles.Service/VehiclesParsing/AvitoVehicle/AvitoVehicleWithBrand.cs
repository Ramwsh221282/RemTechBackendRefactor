using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithBrand(IParsedVehicleBrandSource source, IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleBrand brand = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, brand);
    }
}
