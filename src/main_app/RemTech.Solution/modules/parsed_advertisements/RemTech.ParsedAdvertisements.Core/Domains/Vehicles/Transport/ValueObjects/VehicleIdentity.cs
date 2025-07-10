namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed record VehicleIdentity
{
    private readonly VehicleId _id;

    public VehicleIdentity(VehicleId id) => _id = id;

    public VehicleId Read() => _id;
}
