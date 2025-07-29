using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleIdentity(DbDataReader reader)
{
    public VehicleIdentity Read()
    {
        string id = reader.GetString(reader.GetOrdinal("vehicle_id"));
        return new VehicleIdentity(new VehicleId(new NotEmptyString(id)));
    }
}
