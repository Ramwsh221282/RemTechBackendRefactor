using Npgsql;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal sealed class VehicleModelQuerySpecification(Guid modelId) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = string.Intern("v.model_id = @model_id");
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@model_id", modelId));
    }
}
