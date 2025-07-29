using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

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
