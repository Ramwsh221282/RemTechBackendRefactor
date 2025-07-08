using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

public sealed record ParsedVehicleIdentity
{
    private readonly ParsedVehicleId _id;

    public ParsedVehicleIdentity(ParsedVehicleId id)
    {
        _id = id;
    }

    public ParsedVehicleId ReadId() => _id;

    public static implicit operator string(ParsedVehicleIdentity identity)
    {
        return identity._id;
    }

    public static implicit operator NotEmptyString(ParsedVehicleIdentity identity)
    {
        return identity._id;
    }
}
