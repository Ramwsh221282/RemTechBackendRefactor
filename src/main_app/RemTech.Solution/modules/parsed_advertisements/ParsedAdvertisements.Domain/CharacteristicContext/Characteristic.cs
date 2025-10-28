using ParsedAdvertisements.Domain.CharacteristicContext.ValueObjects;

namespace ParsedAdvertisements.Domain.CharacteristicContext;

public sealed class Characteristic
{
    public CharacteristicId Id { get; }
    public CharacteristicName Name { get; }

    public Characteristic(CharacteristicName name, CharacteristicId? id = null)
    {
        Id = id ?? new CharacteristicId();
        Name = name;
    }
}
