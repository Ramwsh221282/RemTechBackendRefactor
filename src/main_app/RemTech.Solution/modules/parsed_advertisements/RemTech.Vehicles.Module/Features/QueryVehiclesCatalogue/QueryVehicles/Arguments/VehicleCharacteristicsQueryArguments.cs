using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

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
