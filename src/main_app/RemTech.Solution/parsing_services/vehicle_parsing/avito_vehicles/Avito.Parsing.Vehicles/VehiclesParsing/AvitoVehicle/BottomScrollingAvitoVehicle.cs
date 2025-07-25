using Parsing.SDK.ScrapingActions;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicle;

public sealed class BottomScrollingAvitoVehicle(
    IPageAction bottomScroll,
    IAvitoVehicle origin)
    : IAvitoVehicle
{
    public async Task<AvitoVehicleEnvelope> VehicleSource()
    {
        await bottomScroll.Do();
        return await origin.VehicleSource();
    }
}