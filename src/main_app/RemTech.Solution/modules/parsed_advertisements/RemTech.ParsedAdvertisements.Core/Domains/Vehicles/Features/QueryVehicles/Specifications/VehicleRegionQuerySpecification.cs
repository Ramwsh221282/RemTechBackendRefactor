using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed class VehicleRegionQuerySpecification(GeoLocationIdentity identity) : IQueryVehiclesSpecification
{
  public void ApplyTo(VehiclesSqlQuery query)
  {
    string sql = "v.region_id = @region_id";
    Guid id = identity.ReadId();
    query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@region_id", id));
  }
}