using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public abstract record VehicleQueryFilterArgument
{
    public abstract CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    );
}
