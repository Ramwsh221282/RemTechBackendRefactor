using Npgsql;
using RemTech.ParsedAdvertisements.Core.Types.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed class VehicleModelQuerySpecification : IQueryVehiclesSpecification
{
    private readonly VehicleModelIdentity _identity;

    public VehicleModelQuerySpecification(VehicleModelIdentity identity) => _identity = identity;

    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = "v.model_id = @model_id";
        Guid modelId = _identity;
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@model_id", modelId));
    }
}
