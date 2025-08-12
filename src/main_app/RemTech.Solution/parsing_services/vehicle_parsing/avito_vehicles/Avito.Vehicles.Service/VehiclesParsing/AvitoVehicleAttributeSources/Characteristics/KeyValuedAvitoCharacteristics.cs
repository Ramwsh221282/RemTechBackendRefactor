using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class KeyValuedAvitoCharacteristics(IPage page) : IKeyValuedCharacteristicsSource
{
    private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    private const string Mark = "марка";
    private const string Model = "модель";
    private const string Type = "тип техники";
    private const string State = "состояние";
    private const string New = "новое";
    private const string Bu = "Б/у";
    private const string Yes = "Да";
    private const string No = "Нет";
    private const char SplitChar = ':';

    public async Task<CharacteristicsDictionary> Read()
    {
        IElementHandle[] ctxes = await new DefaultOnErrorAvitoCharacteristics(
            new AvitoCharacteristicsSource(page)
        ).Read();
        CharacteristicsDictionary keyValued = new();
        foreach (IElementHandle ctxe in ctxes)
        {
            string pair = await new TextFromWebElement(ctxe).Read();
            string[] parts = pair.Split(SplitChar, SplitOptions);
            string name = parts[0];
            string value = parts[1];
            if (
                name.Contains(Mark, Comparison)
                || name.Contains(Model, Comparison)
                || name.Contains(Type, Comparison)
            )
                continue;
            if (name.Equals(State, Comparison))
            {
                keyValued = value.Contains(New, Comparison)
                    ? keyValued.With(new VehicleCharacteristic(Bu, Yes))
                    : keyValued.With(new VehicleCharacteristic(Bu, No));
                continue;
            }
            keyValued = keyValued.With(new VehicleCharacteristic(name, value));
        }

        return keyValued;
    }
}
