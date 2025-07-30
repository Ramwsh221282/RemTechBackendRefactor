using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed class VehicleModelQuerySpecification(VehicleModelIdentity identity) : IQueryVehiclesSpecification
{
  public void ApplyTo(VehiclesSqlQuery query)
  {
    string sql = "v.model_id = @model_id";
    Guid modelId = identity;
    query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@model_id", modelId));
  }
}