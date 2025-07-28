using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Extensions;

public static class QueryVehiclesSpecificationExtensions
{
    public static CompositeVehicleSpeicification ApplyIfProvided(
        this VehicleQueryFilterArgument? argument,
        CompositeVehicleSpeicification composite)
    {
        return argument == null ? composite : argument.ApplyTo(composite);
    }
}
