using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehicles.Arguments;

public sealed record VehicleCharacteristicsQueryArguments(
    IEnumerable<VehicleCharacteristicQueryArgument> Arguments
) : VehicleQueryFilterArgument
{
    public override CompositeVehicleSpeicification ApplyTo(
        CompositeVehicleSpeicification speicification
    )
    {
        CompositeVehicleSpeicification current = speicification;
        LinkedList<VehicleCharacteristic> characteristics = [];
        foreach (VehicleCharacteristicQueryArgument argument in Arguments)
            characteristics.AddFirst(argument.AsCharacteristic());
        return current.With(new VehicleCharacteristicQuerySpecification(characteristics));
    }
}