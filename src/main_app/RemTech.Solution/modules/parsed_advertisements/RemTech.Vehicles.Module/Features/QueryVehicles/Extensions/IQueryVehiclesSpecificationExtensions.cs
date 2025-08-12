using RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;
using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Extensions;

internal static class QueryVehiclesSpecificationExtensions
{
    internal static CompositeVehicleSpeicification ApplyIfProvided(
        this VehicleQueryFilterArgument? argument,
        CompositeVehicleSpeicification composite
    )
    {
        return argument == null ? composite : argument.ApplyTo(composite);
    }
}
