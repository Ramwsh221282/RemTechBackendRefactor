namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

public sealed record VehicleBrandIdentity
{
    private readonly VehicleBrandId _id;
    private readonly VehicleBrandText _text;

    public VehicleBrandIdentity(VehicleBrandId id, VehicleBrandText text)
    {
        _id = id;
        _text = text;
    }

    public VehicleBrandIdentity(VehicleBrandText text)
    {
        _id = new VehicleBrandId();
        _text = text;
    }

    public VehicleBrandId ReadId() => _id;

    public VehicleBrandText ReadText() => _text;

    public static implicit operator bool(VehicleBrandIdentity? identity)
    {
        if (identity == null)
            return false;
        return identity._id && identity._text;
    }
}
