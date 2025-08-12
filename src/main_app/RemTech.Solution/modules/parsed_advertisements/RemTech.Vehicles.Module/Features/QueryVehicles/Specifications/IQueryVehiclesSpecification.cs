namespace RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

internal interface IQueryVehiclesSpecification
{
    void ApplyTo(IVehiclesSqlQuery query);
}
