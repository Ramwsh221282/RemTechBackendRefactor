using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

public abstract record VehicleQueryFilterArgument
{
    public abstract CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    );
}