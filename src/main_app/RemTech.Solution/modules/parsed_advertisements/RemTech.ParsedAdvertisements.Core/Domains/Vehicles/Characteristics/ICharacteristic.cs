using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public interface ICharacteristic
{
    public CharacteristicIdentity Identify();
}
