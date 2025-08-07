using Parsing.SDK.ScrapingActions;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicle;

public sealed class NavigatingAvitoVehicle(IPageNavigating navigating, IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        await navigating.Do();
        return await origin.VehicleSource();
    }
}
