namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

public sealed record CharacteristicIdentity
{
    private readonly CharacteristicId _id;
    private readonly CharacteristicText _text;

    public CharacteristicIdentity(CharacteristicText text)
        : this(new CharacteristicId(), text) { }

    public CharacteristicIdentity(CharacteristicId id, CharacteristicText text)
    {
        _id = id;
        _text = text;
    }

    public CharacteristicId ReadId() => _id;

    public CharacteristicText ReadText() => _text;
}
