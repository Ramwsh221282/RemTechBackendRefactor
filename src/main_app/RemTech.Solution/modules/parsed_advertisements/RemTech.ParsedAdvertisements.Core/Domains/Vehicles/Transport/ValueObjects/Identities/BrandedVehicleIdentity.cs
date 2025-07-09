using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

public sealed record BrandedVehicleIdentity
{
    private readonly VehicleBrandIdentity _brand;
    private readonly LocationedVehicleIdentity _locationed;

    public BrandedVehicleIdentity(VehicleBrandIdentity brand, LocationedVehicleIdentity locationed)
    {
        _brand = brand;
        _locationed = locationed;
    }

    public BrandedVehicleIdentity(VehicleBrand brand, LocationedVehicleIdentity locationed)
    {
        _brand = brand.Identify();
        _locationed = locationed;
    }

    public LocationedVehicleIdentity WhatLocation() => _locationed;

    public VehicleBrandIdentity ReadBrand() => _brand;
}
