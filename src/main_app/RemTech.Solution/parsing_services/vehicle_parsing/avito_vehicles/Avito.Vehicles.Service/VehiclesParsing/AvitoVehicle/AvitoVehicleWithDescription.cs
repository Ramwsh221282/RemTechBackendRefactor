using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Description;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class AvitoVehicleWithDescription : IAvitoVehicle
{
    private readonly IAvitoDescriptionSource _source;
    private readonly IAvitoVehicle _origin;

    public AvitoVehicleWithDescription(IAvitoDescriptionSource source, IAvitoVehicle origin)
    {
        _source = source;
        _origin = origin;
    }

    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        string description = await _source.Read();
        AvitoVehicleEnvelope origin = await _origin.VehicleSource();
        return new AvitoVehicleEnvelope(origin, description);
    }
}
