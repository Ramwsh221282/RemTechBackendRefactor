using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public abstract record VehicleQueryFilterArgument
{
    public abstract CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    );
}
