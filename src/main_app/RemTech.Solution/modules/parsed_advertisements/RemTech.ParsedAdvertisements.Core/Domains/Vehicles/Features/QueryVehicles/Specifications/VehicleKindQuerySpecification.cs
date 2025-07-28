using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed class VehicleKindQuerySpecification(VehicleKindIdentity identity)
    : IQueryVehiclesSpecification
{
    public void ApplyTo(VehiclesSqlQuery query)
    {
        string sql = "v.kind_id = @kind_id";
        Guid id = identity.ReadId();
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@kind_id", id));
    }
}