using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class DefaultOnErrorCharacteristics : IKeyValuedCharacteristicsSource
{
    private readonly IKeyValuedCharacteristicsSource _origin;

    public DefaultOnErrorCharacteristics(IKeyValuedCharacteristicsSource origin)
    {
        _origin = origin;
    }
    
    public async Task<CharacteristicsDictionary> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new CharacteristicsDictionary();
        }
    }
}