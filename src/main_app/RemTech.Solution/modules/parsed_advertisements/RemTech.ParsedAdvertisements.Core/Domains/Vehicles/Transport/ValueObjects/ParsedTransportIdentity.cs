using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record ParsedTransportIdentity
{
    private readonly ParsedTransportId _id;

    public ParsedTransportIdentity(ParsedTransportId id)
    {
        _id = id;
    }

    public ParsedTransportId ReadId() => _id;

    public static implicit operator string(ParsedTransportIdentity identity)
    {
        return identity._id;
    }

    public static implicit operator NotEmptyString(ParsedTransportIdentity identity)
    {
        return identity._id;
    }
}
