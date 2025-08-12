using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources;

public interface IAvitoCharacteristicsSource
{
    Task<IElementHandle[]> Read();
}
