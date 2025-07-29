using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public sealed record VehicleBrandIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleBrandQuerySpecification(
                new VehicleBrandIdentity(new VehicleBrandId(Id), new VehicleBrandText(string.Empty))
            )
        );
    }
}
