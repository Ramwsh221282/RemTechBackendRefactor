using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery;

public sealed class VehicleBrandBakery : IVehicleBrandBakery
{
    public Status<IVehicleBrand> Bake(IVehicleBrandReceipt receipt)
    {
        return new VehicleBrand(
            new VehicleBrandIdentity(
                new VehicleBrandId(receipt.WhatId()),
                new VehicleBrandText(receipt.WhatName())
            )
        );
    }
}
