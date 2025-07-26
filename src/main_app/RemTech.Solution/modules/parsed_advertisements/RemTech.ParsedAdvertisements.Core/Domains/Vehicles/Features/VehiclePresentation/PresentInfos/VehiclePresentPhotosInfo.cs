using System.Data.Common;
using System.Text.Json;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentPhotosInfo
{
    private readonly string[] _photos;
    public VehiclePresentPhotosInfo() => _photos = [];
    private VehiclePresentPhotosInfo(string[] photos) => _photos = photos;

    public VehiclePresentPhotosInfo RiddenBy(DbDataReader reader)
    {
        string json = reader.GetString(reader.GetOrdinal("vehicle_photos"));
        using JsonDocument document = JsonDocument.Parse(json);
        int length = document.RootElement.GetArrayLength();
        int index = 0;
        string[] photos = new string[length];
        foreach (JsonElement photo in document.RootElement.EnumerateArray())
        {
            photos[index] = photo.GetString()!;
            index++;
        }
        
        return new  VehiclePresentPhotosInfo(photos);
    }
}