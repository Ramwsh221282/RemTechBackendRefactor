using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;

public readonly record struct VehicleModelIdentity
{
    private readonly NotEmptyGuid _id;

    public VehicleModelIdentity() => _id = new NotEmptyGuid(Guid.Empty);

    public VehicleModelIdentity(Guid id) => _id = new NotEmptyGuid(id);

    public VehicleModelIdentity(NotEmptyGuid id) => _id = new NotEmptyGuid();

    public static implicit operator NotEmptyGuid(VehicleModelIdentity identity) => identity._id;
    public static implicit operator Guid(VehicleModelIdentity identity) => identity._id;
    public static implicit operator bool(VehicleModelIdentity identity) => identity._id;
}