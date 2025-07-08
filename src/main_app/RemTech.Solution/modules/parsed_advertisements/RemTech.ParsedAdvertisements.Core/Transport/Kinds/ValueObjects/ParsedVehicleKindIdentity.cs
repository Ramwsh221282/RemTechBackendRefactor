namespace RemTech.ParsedAdvertisements.Core.Transport.Kinds.ValueObjects;

public sealed record ParsedVehicleKindIdentity
{
    private readonly ParsedVehicleKindId _id;

    private readonly ParsedVehicleKindText _text;

    public ParsedVehicleKindIdentity(ParsedVehicleKindId id, ParsedVehicleKindText text)
    {
        _id = id;
        _text = text;
    }

    public ParsedVehicleKindIdentity(ParsedVehicleKindText text)
        : this(new ParsedVehicleKindId(), text) { }

    public ParsedVehicleKindId ReadId() => _id;

    public ParsedVehicleKindText ReadText() => _text;
}
