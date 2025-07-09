using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;

public interface IVehicleBrandReceipt
{
    public NotEmptyString WhatName();
    public NotEmptyGuid WhatId();
}
