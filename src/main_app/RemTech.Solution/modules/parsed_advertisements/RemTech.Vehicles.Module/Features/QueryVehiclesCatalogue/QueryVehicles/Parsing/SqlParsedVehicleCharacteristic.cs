using System.Data.Common;
using RemTech.Vehicles.Module.Types.Characteristics;
using RemTech.Vehicles.Module.Types.Characteristics.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleCharacteristic(DbDataReader reader)
{
    public VehicleCharacteristic Read()
    {
        string name = reader.GetString(reader.GetOrdinal("ctx_name"));
        Guid id = reader.GetGuid(reader.GetOrdinal("ctx_id"));
        string value = reader.GetString(reader.GetOrdinal("ctx_value"));
        string measure = reader.GetString(reader.GetOrdinal("ctx_measure"));
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
