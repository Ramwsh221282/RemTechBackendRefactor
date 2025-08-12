using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal sealed class VehicleRegionQuerySpecification(Guid locationId) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = string.Intern("v.geo_id = @region_id");
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@region_id", locationId));
    }
}
