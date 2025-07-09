using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class Characteristic : ICharacteristic
{
    private readonly CharacteristicIdentity _identity;

    public Characteristic(CharacteristicIdentity identity)
    {
        _identity = identity;
    }

    public CharacteristicIdentity Identify()
    {
        return _identity;
    }
}
