using System.Data.Common;
using System.Text.Json;
using RemTech.Vehicles.Module.Types.Characteristics;
using RemTech.Vehicles.Module.Types.Characteristics.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleCharacteristic(JsonElement source)
{
    public VehicleCharacteristic Read()
    {
        string name = source.GetProperty("ctx_name").GetString()!;
        string value = source.GetProperty("ctx_value").GetString()!;
        string measure = source.GetProperty("ctx_measure").GetString()!;
        Guid id = source.GetProperty("ctx_id").GetGuid();
        CharacteristicIdentity identity = new(
            new CharacteristicId(id),
            new CharacteristicText(name)
        );
        VehicleCharacteristicValue vehicleCtxValue = new VehicleCharacteristicValue(value);
        VehicleCharacteristic characteristic = new(
            new Characteristic(identity, new CharacteristicMeasure(measure)),
            vehicleCtxValue
        );
        return characteristic;
    }
}
