using System.Data.Common;
using RemTech.Vehicles.Module.Types.Kinds;
using RemTech.Vehicles.Module.Types.Kinds.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleKind(DbDataReader reader)
{
    public VehicleKind Read()
    {
        Guid kindId = reader.GetGuid(reader.GetOrdinal("kind_id"));
        string kindName = reader.GetString(reader.GetOrdinal("kind_name"));
        VehicleKindIdentity identity = new(
            new VehicleKindId(kindId),
            new VehicleKindText(kindName)
        );
        return new VehicleKind(identity);
    }
}
