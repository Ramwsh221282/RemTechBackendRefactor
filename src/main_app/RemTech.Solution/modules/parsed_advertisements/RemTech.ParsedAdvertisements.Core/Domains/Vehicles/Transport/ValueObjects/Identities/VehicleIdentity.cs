namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

public sealed record VehicleIdentity
{
    private readonly VehicleIdentityOfKind _kinded;

    public VehicleIdentity(VehicleIdentityOfKind kinded)
    {
        _kinded = kinded;
    }

    public VehicleIdentityOfKind WhatKind() => _kinded;
}
