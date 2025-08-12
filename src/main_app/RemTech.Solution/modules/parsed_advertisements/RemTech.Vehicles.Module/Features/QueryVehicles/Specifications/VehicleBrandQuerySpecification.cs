using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal sealed class VehicleBrandQuerySpecification(Guid brandId) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = string.Intern("v.brand_id = @brand_id");
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@brand_id", brandId));
    }
}
