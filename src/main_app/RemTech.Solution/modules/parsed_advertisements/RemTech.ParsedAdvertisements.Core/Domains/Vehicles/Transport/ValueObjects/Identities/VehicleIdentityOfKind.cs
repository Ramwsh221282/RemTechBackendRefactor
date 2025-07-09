using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

public sealed class VehicleIdentityOfKind
{
    private readonly VehicleKindIdentity _kindIdentity;
    private readonly BrandedVehicleIdentity _branded;

    public VehicleIdentityOfKind(VehicleKindIdentity kindIdentity, BrandedVehicleIdentity branded)
    {
        _kindIdentity = kindIdentity;
        _branded = branded;
    }

    public VehicleIdentityOfKind(VehicleKind kind, BrandedVehicleIdentity branded)
    {
        _kindIdentity = kind.Identify();
        _branded = branded;
    }

    public VehicleKindIdentity ReadKind() => _kindIdentity;

    public BrandedVehicleIdentity WhatBrand() => _branded;
}
