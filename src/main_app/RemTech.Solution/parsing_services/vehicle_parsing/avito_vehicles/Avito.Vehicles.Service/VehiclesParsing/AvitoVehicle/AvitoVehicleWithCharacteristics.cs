using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithCharacteristics(
    IKeyValuedCharacteristicsSource source,
    IAvitoVehicle origin
) : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource() =>
        new(await origin.VehicleSource(), await source.Read());
}
