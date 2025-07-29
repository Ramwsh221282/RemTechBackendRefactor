using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.QueryVehiclesCatalogue.QueryVehicles.Arguments;

public sealed record VehicleCharacteristicQueryArgument(Guid Id, string Name, string Value)
{
    public VehicleCharacteristic AsCharacteristic()
    {
        CharacteristicIdentity identity = new(
            new CharacteristicId(Id),
            new CharacteristicText(Name)
        );
        Characteristic characteristic = new(identity);
        return new VehicleCharacteristic(characteristic, new VehicleCharacteristicValue(Value));
    }
}
