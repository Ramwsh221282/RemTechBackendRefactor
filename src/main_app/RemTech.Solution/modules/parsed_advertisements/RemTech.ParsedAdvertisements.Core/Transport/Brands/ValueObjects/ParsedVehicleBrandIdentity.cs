namespace RemTech.ParsedAdvertisements.Core.Transport.Brands.ValueObjects;

public sealed record ParsedVehicleBrandIdentity
{
    private readonly ParsedVehicleBrandId _id;
    private readonly ParsedVehicleBrandText _text;

    public ParsedVehicleBrandIdentity(ParsedVehicleBrandId id, ParsedVehicleBrandText text)
    {
        _id = id;
        _text = text;
    }

    public ParsedVehicleBrandIdentity(ParsedVehicleBrandText text)
    {
        _id = new ParsedVehicleBrandId();
        _text = text;
    }

    public ParsedVehicleBrandId ReadId() => _id;

    public ParsedVehicleBrandText ReadText() => _text;
}
