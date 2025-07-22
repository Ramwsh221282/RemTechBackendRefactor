using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleIdentities;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithIdentity(
    IParsedVehicleIdentitySource source,
    IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleIdentity identity = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, identity);
    }
}