using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Delegates;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary.Types;

public sealed record VehicleCharacteristicsDictionaryEntry(
    Guid Id,
    string Name,
    string Measure,
    VehicleCharacteristicsDictionaryEntryValues Values
)
{
    public static ReadVehicleCharacteristicsDictionaryEntry Read() =>
        (dataReader) =>
        {
            Guid id = dataReader.GetGuid(dataReader.GetOrdinal("ctx_id"));
            string name = dataReader.GetString(dataReader.GetOrdinal("ctx_text"));
            string measure = dataReader.GetString(dataReader.GetOrdinal("ctx_measure"));
            string value = dataReader.GetString(dataReader.GetOrdinal("ctx_value"));
            return new VehicleCharacteristicsDictionaryEntry(
                id,
                name,
                measure,
                new VehicleCharacteristicsDictionaryEntryValues([value])
            );
        };
}
