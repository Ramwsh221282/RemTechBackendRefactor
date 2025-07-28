namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

public interface IQueryVehiclesSpecification
{
    void ApplyTo(VehiclesSqlQuery query);
}