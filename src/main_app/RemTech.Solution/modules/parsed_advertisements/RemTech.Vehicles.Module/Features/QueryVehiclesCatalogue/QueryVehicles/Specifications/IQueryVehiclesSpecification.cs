namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

public interface IQueryVehiclesSpecification
{
    void ApplyTo(IVehiclesSqlQuery query);
}
