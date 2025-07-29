using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public sealed class VehiclePriceQuerySpecification(
    double? priceTo,
    double? priceFrom,
    bool? isNds = null
) : IQueryVehiclesSpecification
{
    public void ApplyTo(IVehiclesSqlQuery query)
    {
        if (priceTo.HasValue)
            query.AcceptFilter(
                "v.price <= @priceTo",
                new NpgsqlParameter<double>("@priceTo", priceTo.Value)
            );
        if (priceFrom.HasValue)
            query.AcceptFilter(
                "v.price >= @priceFrom",
                new NpgsqlParameter<double>("@priceFrom", priceFrom.Value)
            );
        if (isNds.HasValue)
            query.AcceptFilter(
                "v.is_nds = @is_nds",
                new NpgsqlParameter<bool>("@is_nds", isNds.Value)
            );
    }
}
