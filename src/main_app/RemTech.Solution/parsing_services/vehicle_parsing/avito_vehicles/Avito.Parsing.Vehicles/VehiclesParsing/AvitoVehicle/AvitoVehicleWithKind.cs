using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithKind(
    IParsedVehicleKindSource source,
    IAvitoVehicle origin) :
    IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleKind kind = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, kind);
    }
}