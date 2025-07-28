using System.Data.Common;
using System.Text.Json;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleCharacteristics(DbDataReader reader)
{
    public VehicleCharacteristics Read()
    {
        string json = reader.GetString(reader.GetOrdinal("vehicle_characteristics"));
        using JsonDocument document = JsonDocument.Parse(json);
        LinkedList<VehicleCharacteristic> characteristics = [];
        foreach (JsonElement ctx in document.RootElement.EnumerateArray())
            characteristics.AddFirst(new SqlParsedVehicleCharacteristic(ctx).Read());
        return new VehicleCharacteristics(characteristics);
    }
}