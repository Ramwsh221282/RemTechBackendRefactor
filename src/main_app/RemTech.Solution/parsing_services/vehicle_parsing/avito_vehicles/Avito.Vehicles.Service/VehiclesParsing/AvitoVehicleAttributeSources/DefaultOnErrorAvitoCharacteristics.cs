using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources;

public sealed class DefaultOnErrorAvitoCharacteristics : IAvitoCharacteristicsSource
{
    private readonly IAvitoCharacteristicsSource _origin;

    public DefaultOnErrorAvitoCharacteristics(IAvitoCharacteristicsSource origin)
    {
        _origin = origin;
    }

    public async Task<IElementHandle[]> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return [];
        }
    }
}
