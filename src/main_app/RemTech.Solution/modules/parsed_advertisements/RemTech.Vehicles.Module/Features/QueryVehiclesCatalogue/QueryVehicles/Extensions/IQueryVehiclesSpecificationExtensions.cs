using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Extensions;

public static class QueryVehiclesSpecificationExtensions
{
    public static CompositeVehicleSpeicification ApplyIfProvided(
        this VehicleQueryFilterArgument? argument,
        CompositeVehicleSpeicification composite
    )
    {
        return argument == null ? composite : argument.ApplyTo(composite);
    }
}
