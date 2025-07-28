using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Parsing;

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
            new GeolocationText(kind));
        return new GeoLocation(identity);
    }
}