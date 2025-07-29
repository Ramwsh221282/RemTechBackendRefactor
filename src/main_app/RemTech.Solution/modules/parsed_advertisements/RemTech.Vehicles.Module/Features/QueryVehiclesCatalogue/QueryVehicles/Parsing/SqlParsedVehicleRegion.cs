using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.GeoLocations;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Parsing;

public sealed class SqlParsedVehicleRegion(DbDataReader reader)
{
    public GeoLocation Read()
    {
        Guid id = reader.GetGuid(reader.GetOrdinal("geo_id"));
        string name = reader.GetString(reader.GetOrdinal("geo_text"));
        string kind = reader.GetString(reader.GetOrdinal("geo_kind"));
        GeoLocationIdentity identity = new(
            new GeoLocationId(new NotEmptyGuid(id)),
            new GeolocationText(name),
            new GeolocationText(kind)
        );
        return new GeoLocation(identity);
    }
}
