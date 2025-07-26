using System.Data.Common;
using System.Text.Json;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.PresentInfos;

public sealed class VehiclePresentCharacteristicsCollection
{
    private readonly VehiclePresentCharacteristicInfo[] _characteristics;

    public VehiclePresentCharacteristicsCollection() => _characteristics = [];
    private VehiclePresentCharacteristicsCollection(VehiclePresentCharacteristicInfo[] characteristics) => 
        _characteristics = characteristics;

    public VehiclePresentCharacteristicsCollection RiddenBy(DbDataReader reader)
    {
        string json = reader.GetString(reader.GetOrdinal("vehicle_characteristics"));
        using JsonDocument document = JsonDocument.Parse(json);
        int length = document.RootElement.GetArrayLength();
        int index = 0;
        VehiclePresentCharacteristicInfo[] characteristics = new VehiclePresentCharacteristicInfo[length];
        foreach (JsonElement ctx in document.RootElement.EnumerateArray())
        {
            characteristics[index] = new VehiclePresentCharacteristicInfo().RiddenBy(ctx);
            index++;
        }

        return new VehiclePresentCharacteristicsCollection(characteristics);
    }
}