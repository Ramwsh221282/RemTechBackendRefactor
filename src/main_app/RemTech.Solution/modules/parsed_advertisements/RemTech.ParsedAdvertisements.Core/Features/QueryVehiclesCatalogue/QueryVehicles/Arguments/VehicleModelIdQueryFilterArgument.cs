using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Types.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public sealed record VehicleModelIdQueryFilterArgument(Guid Id) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        return speicification.With(
            new VehicleModelQuerySpecification(new VehicleModelIdentity(Id))
        );
    }
}
