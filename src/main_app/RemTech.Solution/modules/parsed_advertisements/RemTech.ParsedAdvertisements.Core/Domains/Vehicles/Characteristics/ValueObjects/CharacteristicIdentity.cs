using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

public sealed record CharacteristicIdentity
{
    private readonly CharacteristicId _id;
    private readonly CharacteristicText _text;

    public CharacteristicIdentity()
    {
        _id = new CharacteristicId();
        _text = new CharacteristicText(new NotEmptyString(string.Empty));
    }
    
    public CharacteristicIdentity(CharacteristicText text)
        : this(new CharacteristicId(), text) { }

    public CharacteristicIdentity(CharacteristicId id, CharacteristicText text)
    {
        _id = id;
        _text = text;
    }

    public CharacteristicId ReadId() => _id;

    public CharacteristicText ReadText() => _text;

    public static implicit operator bool(CharacteristicIdentity? identity)
    {
        return identity != null && (identity._text && identity._id);
    }
}
