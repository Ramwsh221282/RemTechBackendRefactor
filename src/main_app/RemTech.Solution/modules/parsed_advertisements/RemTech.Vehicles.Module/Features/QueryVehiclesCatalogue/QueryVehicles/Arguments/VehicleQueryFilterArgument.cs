using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public abstract record VehicleQueryFilterArgument
{
    public abstract CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    );
}
