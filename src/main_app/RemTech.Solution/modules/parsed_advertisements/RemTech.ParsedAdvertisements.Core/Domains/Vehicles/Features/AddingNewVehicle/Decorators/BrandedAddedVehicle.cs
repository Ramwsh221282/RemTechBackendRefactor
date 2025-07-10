using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class BrandedAddedVehicle(
    IVehicleBrands brands,
    string? brandName,
    IAddedVehicle origin
) : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add)
    {
        Status<VehicleBrandEnvelope> brand = brands.Add(brandName);
        return brand.IsFailure
            ? brand.Error
            : origin.Added(new AddVehicle(new BrandedVehicle(brand.Value, add.WhatVehicle())));
    }
}
