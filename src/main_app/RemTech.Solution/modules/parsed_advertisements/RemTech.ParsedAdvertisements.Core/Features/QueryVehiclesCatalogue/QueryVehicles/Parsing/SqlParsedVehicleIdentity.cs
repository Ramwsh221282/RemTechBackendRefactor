using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleIdentity(DbDataReader reader)
{
    public VehicleIdentity Read()
    {
        string id = reader.GetString(reader.GetOrdinal("vehicle_id"));
        return new VehicleIdentity(new VehicleId(new NotEmptyString(id)));
    }
}
