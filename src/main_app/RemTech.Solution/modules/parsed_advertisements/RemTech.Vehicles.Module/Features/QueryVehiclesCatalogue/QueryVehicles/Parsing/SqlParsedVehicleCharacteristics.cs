using System.Data.Common;
using System.Text.Json;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleCharacteristics(DbDataReader reader)
{
    public VehicleCharacteristics Read()
    {
        string json = reader.GetString(reader.GetOrdinal("characteristics"));
        using JsonDocument document = JsonDocument.Parse(json);
        int length = document.RootElement.GetArrayLength();
        VehicleCharacteristic[] characteristics = new VehicleCharacteristic[length];
        int index = 0;
        foreach (JsonElement jsonElement in document.RootElement.EnumerateArray())
        {
            characteristics[index] = new SqlParsedVehicleCharacteristic(jsonElement).Read();
            index++;
        }
        return new VehicleCharacteristics(characteristics);
    }
}
