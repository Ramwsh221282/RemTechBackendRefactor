using System.Data.Common;
using System.Text.Json;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehiclePhotos(DbDataReader reader)
{
    public VehiclePhotos Read()
    {
        string json = reader.GetString(reader.GetOrdinal("vehicle_photos"));
        using JsonDocument document = JsonDocument.Parse(json);
        LinkedList<VehiclePhoto> photos = [];
        foreach (JsonElement photo in document.RootElement.EnumerateArray())
            photos.AddFirst(new SqlParsedVehiclePhoto(photo).Read());
        return new VehiclePhotos(photos);
    }
}
