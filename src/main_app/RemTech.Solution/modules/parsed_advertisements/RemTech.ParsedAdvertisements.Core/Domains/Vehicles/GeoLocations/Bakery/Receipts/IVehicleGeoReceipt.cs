using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;

public interface IVehicleGeoReceipt
{
    public NotEmptyGuid WhatId();
    public NotEmptyString WhatText();
}
