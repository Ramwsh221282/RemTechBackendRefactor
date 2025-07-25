using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithGeo(IParsedVehicleGeoSource source, IAvitoVehicle origin) : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleGeo geo = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, geo);
    }
}