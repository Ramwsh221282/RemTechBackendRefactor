using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

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