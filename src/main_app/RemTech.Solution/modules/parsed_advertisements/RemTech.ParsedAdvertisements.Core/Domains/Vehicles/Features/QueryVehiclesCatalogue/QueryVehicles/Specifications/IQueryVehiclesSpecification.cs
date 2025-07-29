using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.Shared;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public interface IQueryVehiclesSpecification
{
    void ApplyTo(IVehiclesSqlQuery query);
}
