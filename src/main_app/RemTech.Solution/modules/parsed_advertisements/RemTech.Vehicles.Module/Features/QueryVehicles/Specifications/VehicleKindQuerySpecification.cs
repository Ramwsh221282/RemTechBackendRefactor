using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal sealed class VehicleKindQuerySpecification(Guid categoryId) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = string.Intern("v.kind_id = @kind_id");
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@kind_id", categoryId));
    }
}
