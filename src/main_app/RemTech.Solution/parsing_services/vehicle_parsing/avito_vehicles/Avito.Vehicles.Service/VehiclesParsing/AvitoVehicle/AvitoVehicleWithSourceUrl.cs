using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleSources;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithSourceUrl(IParsedVehicleUrlSource source, IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleUrl sourceUrl = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, sourceUrl);
    }
}
