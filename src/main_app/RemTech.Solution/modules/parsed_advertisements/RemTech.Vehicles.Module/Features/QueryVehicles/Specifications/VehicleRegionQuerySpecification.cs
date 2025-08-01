using Npgsql;
using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

public sealed class VehicleRegionQuerySpecification : IQueryVehiclesSpecification
{
    private readonly GeoLocationIdentity _identity;

    public VehicleRegionQuerySpecification(GeoLocationIdentity identity)
    {
        _identity = identity;
    }

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = "v.geo_id = @region_id";
        Guid id = _identity.ReadId();
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@region_id", id));
    }
}
