namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

public interface IQueryVehiclesSpecification
{
    void ApplyTo(IVehiclesSqlQuery query);
}
