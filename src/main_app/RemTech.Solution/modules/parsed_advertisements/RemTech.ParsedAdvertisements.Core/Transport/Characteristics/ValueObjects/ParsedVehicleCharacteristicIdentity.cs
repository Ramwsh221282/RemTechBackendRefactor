namespace RemTech.ParsedAdvertisements.Core.Transport.Characteristics.ValueObjects;

public sealed record ParsedVehicleCharacteristicIdentity
{
    private readonly ParsedVehicleCharacteristicId _id;
    private readonly ParsedVehicleCharacteristicText _text;

    public ParsedVehicleCharacteristicIdentity(ParsedVehicleCharacteristicText text)
        : this(new ParsedVehicleCharacteristicId(), text) { }

    public ParsedVehicleCharacteristicIdentity(
        ParsedVehicleCharacteristicId id,
        ParsedVehicleCharacteristicText text
    )
    {
        _id = id;
        _text = text;
    }

    public ParsedVehicleCharacteristicId ReadId() => _id;

    public ParsedVehicleCharacteristicText ReadText() => _text;
}
