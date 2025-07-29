using RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

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
