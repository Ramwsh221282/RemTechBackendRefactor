using System.Data.Common;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleCharacteristics(DbDataReader reader)
{
    public VehicleCharacteristics Read()
    {
        VehicleCharacteristic[] characteristics = new VehicleCharacteristic[1];
        characteristics[0] = new SqlParsedVehicleCharacteristic(reader).Read();
        return new VehicleCharacteristics(characteristics);
    }
}
