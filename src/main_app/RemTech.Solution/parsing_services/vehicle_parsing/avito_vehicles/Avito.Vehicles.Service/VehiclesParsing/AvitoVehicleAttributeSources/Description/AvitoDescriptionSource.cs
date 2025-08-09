using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Description;

public sealed class AvitoDescriptionSource(IPage page) : IAvitoDescriptionSource
{
    public async Task<string> Read()
    {
        AvitoDescriptionParts parts = new AvitoDescriptionParts(page);
        string[] texts = await parts.Read();
        string finalText = string.Join(' ', texts);
        return finalText;
    }
}
