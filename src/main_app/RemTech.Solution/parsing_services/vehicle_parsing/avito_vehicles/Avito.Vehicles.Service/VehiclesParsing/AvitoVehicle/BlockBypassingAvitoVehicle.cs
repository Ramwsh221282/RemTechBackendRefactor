using Parsing.Avito.Common.BypassFirewall;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class BlockBypassingAvitoVehicle(IAvitoBypassFirewall bypass, IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource() =>
        !await bypass.Read() ? new AvitoVehicleEnvelope() : await origin.VehicleSource();
}
