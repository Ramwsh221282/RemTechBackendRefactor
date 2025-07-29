namespace RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects;

public sealed record VehicleIdentity
{
    private readonly VehicleId _id;

    public VehicleIdentity(VehicleId id) => _id = id;

    public VehicleId Read() => _id;

    public static implicit operator bool(VehicleIdentity? identity)
    {
        return identity != null && identity._id;
    }
}
