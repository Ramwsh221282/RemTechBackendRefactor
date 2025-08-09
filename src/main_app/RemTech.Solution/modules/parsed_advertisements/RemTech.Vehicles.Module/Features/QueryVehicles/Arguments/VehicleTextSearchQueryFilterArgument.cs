using RemTech.Vehicles.Module.Features.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public sealed record VehicleTextSearchQueryFilterArgument(string TextSearch)
    : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        speicification = speicification.With(new TextSearchQuerySpecification(TextSearch));
        return speicification;
    }
}
