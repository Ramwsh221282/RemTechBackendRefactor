namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

public sealed record VehicleKindIdentity
{
    private readonly VehicleKindId _id;

    private readonly VehicleKindText _text;

    public VehicleKindIdentity(VehicleKindId id, VehicleKindText text)
    {
        _id = id;
        _text = text;
    }

    public VehicleKindIdentity(VehicleKindText text)
        : this(new VehicleKindId(), text) { }

    public VehicleKindId ReadId() => _id;

    public VehicleKindText ReadText() => _text;

    public static implicit operator bool(VehicleKindIdentity? identity)
    {
        return identity != null && identity._text && identity._id;
    }
}
