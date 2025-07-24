using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class KeyValuedAvitoCharacteristics(IPage page) : IKeyValuedCharacteristicsSource 
{
    private readonly StringComparison _comparison = StringComparison.InvariantCultureIgnoreCase;
    private readonly StringSplitOptions _splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    
    public async Task<CharacteristicsDictionary> Read()
    {
        IElementHandle[] ctxes = await new DefaultOnErrorAvitoCharacteristics(
                new AvitoCharacteristicsSource(page))
            .Read();
        CharacteristicsDictionary keyValued = new();
        foreach (IElementHandle ctxe in ctxes)
        {
            string pair = await new TextFromWebElement(ctxe).Read();
            string[] parts = pair.Split(':', _splitOptions);
            string name = parts[0];
            string value = parts[1];
            if (name.Contains("марка", _comparison) ||
                name.Contains("модель", _comparison) ||
                name.Contains("тип техники", _comparison))
                continue;
            if (name.Equals("состояние", _comparison))
            {
                keyValued = value.Contains("новое", _comparison)
                    ? keyValued.With(new VehicleCharacteristic("Б/у", "Да"))
                    : keyValued.With(new VehicleCharacteristic("Б/у", "Нет"));
                continue;
            }
            keyValued = keyValued.With(new VehicleCharacteristic(name, value));
        }
        
        return keyValued;
    }
}