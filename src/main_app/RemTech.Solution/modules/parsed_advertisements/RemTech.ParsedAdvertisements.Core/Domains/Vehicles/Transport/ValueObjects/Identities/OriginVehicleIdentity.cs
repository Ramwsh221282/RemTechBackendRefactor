namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

public sealed class OriginVehicleIdentity
{
    private readonly VehicleId _id;

    public OriginVehicleIdentity(VehicleId id) => _id = id;

    public VehicleId ReadId() => _id;
}
