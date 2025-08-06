using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithModel(IParsedVehicleModelSource source, IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        AvitoVehicleEnvelope fromOrigin = await origin.VehicleSource();
        ParsedVehicleModel model = await source.Read();
        return new AvitoVehicleEnvelope(fromOrigin, model);
    }
}
