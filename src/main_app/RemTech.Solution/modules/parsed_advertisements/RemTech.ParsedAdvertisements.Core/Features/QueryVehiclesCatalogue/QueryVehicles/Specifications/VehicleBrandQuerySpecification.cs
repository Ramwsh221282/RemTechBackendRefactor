using Npgsql;
using RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed class VehicleBrandQuerySpecification(VehicleBrandIdentity identity)
    : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = "v.brand_id = @brand_id";
        Guid id = identity.ReadId();
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@brand_id", id));
    }
}
