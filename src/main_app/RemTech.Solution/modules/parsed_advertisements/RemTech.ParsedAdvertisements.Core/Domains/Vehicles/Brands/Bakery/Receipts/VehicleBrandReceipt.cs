using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;

public sealed record VehicleBrandReceipt : IVehicleBrandReceipt
{
    private readonly NotEmptyString _brandName;
    private readonly NotEmptyGuid _brandId;

    public VehicleBrandReceipt(NotEmptyString brandName, NotEmptyGuid brandId)
    {
        _brandName = brandName;
        _brandId = brandId;
    }

    public NotEmptyString WhatName()
    {
        return _brandName;
    }

    public NotEmptyGuid WhatId()
    {
        return _brandId;
    }

    public Status<IVehicleBrand> Baked(IVehicleBrandBakery bakery)
    {
        return bakery.Bake(this);
    }
}
