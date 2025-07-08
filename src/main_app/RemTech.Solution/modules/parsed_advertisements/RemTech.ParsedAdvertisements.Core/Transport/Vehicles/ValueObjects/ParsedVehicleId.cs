using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedSourceDetails.Identities;

namespace RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

public readonly record struct ParsedVehicleId
{
    private readonly ParsedItemId _id;

    public ParsedVehicleId(ParsedItemId id)
    {
        _id = id;
    }

    public ParsedVehicleId(NotEmptyString id)
        : this(new ParsedItemId(id)) { }

    public ParsedVehicleId(string? id)
        : this(new NotEmptyString(id)) { }

    public static implicit operator string(ParsedVehicleId id) => id._id;

    public static implicit operator bool(ParsedVehicleId id) => id._id;

    public static implicit operator NotEmptyString(ParsedVehicleId id) => id._id;
}
