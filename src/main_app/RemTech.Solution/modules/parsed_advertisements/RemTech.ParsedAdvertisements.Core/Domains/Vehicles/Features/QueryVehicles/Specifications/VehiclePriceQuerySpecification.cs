using Npgsql;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public sealed class VehiclePriceQuerySpecification(double? priceTo, double? priceFrom, bool isNds)
  : IQueryVehiclesSpecification
{
  public void ApplyTo(VehiclesSqlQuery query)
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
    query.AcceptFilter("v.is_nds = @isNds", new NpgsqlParameter<bool>("v.is_nds", isNds));
  }
}