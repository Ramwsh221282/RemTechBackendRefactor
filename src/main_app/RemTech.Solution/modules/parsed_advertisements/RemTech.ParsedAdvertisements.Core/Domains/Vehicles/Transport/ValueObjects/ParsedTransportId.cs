using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public readonly record struct ParsedTransportId
{
    private readonly NotEmptyString _id;

    public ParsedTransportId(NotEmptyString id)
    {
        _id = id;
    }

    public ParsedTransportId(string? id)
        : this(new NotEmptyString(id)) { }

    public static implicit operator string(ParsedTransportId id) => id._id;

    public static implicit operator bool(ParsedTransportId id) => id._id;

    public static implicit operator NotEmptyString(ParsedTransportId id) => id._id;
}
