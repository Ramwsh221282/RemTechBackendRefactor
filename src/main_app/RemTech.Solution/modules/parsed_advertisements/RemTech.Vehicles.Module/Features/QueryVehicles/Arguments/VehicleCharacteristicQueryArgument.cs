using RemTech.Vehicles.Module.Types.Characteristics;
using RemTech.Vehicles.Module.Types.Characteristics.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Features.QueryVehicles.Arguments;

public sealed record VehicleCharacteristicQueryArgument(Guid Id, string Name, string Value)
{
    internal VehicleCharacteristic AsCharacteristic()
    {
        CharacteristicIdentity identity = new(
            new CharacteristicId(Id),
            new CharacteristicText(Name)
        );
        Characteristic characteristic = new(identity);
        return new VehicleCharacteristic(characteristic, new VehicleCharacteristicValue(Value));
    }
}
