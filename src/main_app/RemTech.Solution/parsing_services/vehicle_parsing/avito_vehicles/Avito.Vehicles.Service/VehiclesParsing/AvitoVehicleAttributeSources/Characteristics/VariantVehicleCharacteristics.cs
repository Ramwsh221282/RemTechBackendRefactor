using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class VariantVehicleCharacteristics : IKeyValuedCharacteristicsSource
{
    private readonly Queue<IKeyValuedCharacteristicsSource> _sources;

    public VariantVehicleCharacteristics()
    {
        _sources = [];
    }

    public VariantVehicleCharacteristics With(IKeyValuedCharacteristicsSource source)
    {
        _sources.Enqueue(source);
        return this;
    }

    public async Task<CharacteristicsDictionary> Read()
    {
        CharacteristicsDictionary main = new();
        while (_sources.Count > 0)
        {
            IKeyValuedCharacteristicsSource source = _sources.Dequeue();
            CharacteristicsDictionary other = await source.Read();
            main = main.FromOther(other);
        }
        return main;
    }
}
