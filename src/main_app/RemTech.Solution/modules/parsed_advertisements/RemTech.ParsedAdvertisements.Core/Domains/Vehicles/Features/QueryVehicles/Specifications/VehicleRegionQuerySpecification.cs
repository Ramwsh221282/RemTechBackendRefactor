using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed class VehicleRegionQuerySpecification : IQueryVehiclesSpecification
{
    private readonly GeoLocationIdentity _identity;

    public VehicleRegionQuerySpecification(GeoLocationIdentity identity)
    {
        _identity = identity;
    }

    public void ApplyTo(VehiclesSqlQuery query)
    {
        string sql = "v.geo_id = @region_id";
        Guid id = _identity.ReadId();
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@region_id", id));
    }
}