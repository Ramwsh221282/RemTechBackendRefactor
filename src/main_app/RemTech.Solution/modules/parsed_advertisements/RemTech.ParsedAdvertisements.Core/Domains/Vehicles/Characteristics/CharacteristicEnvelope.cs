using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public abstract class CharacteristicEnvelope(CharacteristicIdentity identity) : ICharacteristic
{
    private readonly CharacteristicIdentity _identity = identity;

    public CharacteristicIdentity Identify() => _identity;
}
