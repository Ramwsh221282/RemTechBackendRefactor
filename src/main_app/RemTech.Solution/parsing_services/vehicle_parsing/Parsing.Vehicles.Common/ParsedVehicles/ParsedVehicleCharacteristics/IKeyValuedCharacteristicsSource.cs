namespace Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;

public interface IKeyValuedCharacteristicsSource
{
    Task<CharacteristicsDictionary> Read();
}