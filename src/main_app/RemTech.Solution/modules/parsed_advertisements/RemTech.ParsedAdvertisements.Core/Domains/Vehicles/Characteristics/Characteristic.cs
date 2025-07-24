using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class Characteristic : ICharacteristic
{
    private readonly CharacteristicIdentity _identity;
    private readonly CharacteristicMeasure _measure;

    public Characteristic()
    {
        _identity = new CharacteristicIdentity();
        _measure = new CharacteristicMeasure();
    }

    public Characteristic(CharacteristicIdentity identity, CharacteristicMeasure measure)
    {
        _identity = identity;
        _measure = measure;
    }

    public Characteristic(Characteristic origin)
    {
        _identity = origin._identity;
        _measure = origin._measure;
    }

    public Characteristic(Characteristic origin, CharacteristicMeasure measure)
    : this(origin)
    {
        _measure = measure;
    }

    public Characteristic(CharacteristicIdentity identity)
    {
        _identity = identity;
        _measure = new CharacteristicMeasure();
    }
    
    public CharacteristicIdentity Identify() => _identity;
    public CharacteristicMeasure Measure() => _measure;

    public ValuedCharacteristic Print(NotEmptyString value)
    {
        return new ValuedCharacteristic(this, value);
    }
}
