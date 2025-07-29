using Npgsql;
using RemTech.ParsedAdvertisements.Core.Types.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed class VehicleKindQuerySpecification(VehicleKindIdentity identity)
    : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        string sql = "v.kind_id = @kind_id";
        Guid id = identity.ReadId();
        query.AcceptFilter(sql, new NpgsqlParameter<Guid>("@kind_id", id));
    }
}
