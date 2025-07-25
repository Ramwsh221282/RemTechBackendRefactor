using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithSourceUrl(IParsedVehicleUrlSource source, IAvitoVehicle origin) : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleUrl sourceUrl = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, sourceUrl);
    }
}