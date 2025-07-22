using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class KeyValuedAvitoCharacteristics(IPage page) : IKeyValuedCharacteristicsSource 
{
    public async Task<KeyValueVehicleCharacteristics> Read()
    {
        IElementHandle[] ctxes = await new AvitoCharacteristicsSource(page).Read();
        KeyValueVehicleCharacteristics keyValued = new();
        foreach (IElementHandle ctxe in ctxes)
        {
            string pair = await new TextFromWebElement(ctxe).Read();
            string[] parts = pair.Split(':', StringSplitOptions.TrimEntries);
            string name = parts[0];
            string value = parts[1];
            if (name.Contains("марка", StringComparison.CurrentCultureIgnoreCase) ||
                name.Contains("модель", StringComparison.CurrentCultureIgnoreCase) ||
                name.Contains("тип техники", StringComparison.CurrentCultureIgnoreCase))
                continue;
                
            keyValued = keyValued.With((name, value));
        }
        
        return keyValued;
    }
}