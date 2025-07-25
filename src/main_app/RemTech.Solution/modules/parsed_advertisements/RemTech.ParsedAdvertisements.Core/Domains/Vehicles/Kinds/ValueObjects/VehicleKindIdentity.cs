using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

public sealed record VehicleKindIdentity
{
    private readonly VehicleKindId _id;

    private readonly VehicleKindText _text;

    public VehicleKindIdentity()
    {
        _id = new VehicleKindId(new NotEmptyGuid(Guid.Empty));
        _text = new VehicleKindText(new NotEmptyString(string.Empty));
    }
    
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
