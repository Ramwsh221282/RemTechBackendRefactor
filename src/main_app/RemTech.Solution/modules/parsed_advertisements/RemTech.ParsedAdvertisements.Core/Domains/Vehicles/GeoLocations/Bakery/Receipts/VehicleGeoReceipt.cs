using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;

public sealed class VehicleGeoReceipt : IVehicleGeoReceipt
{
    private readonly NotEmptyGuid _id;
    private readonly NotEmptyString _text;

    public VehicleGeoReceipt(NotEmptyGuid id, NotEmptyString text)
    {
        _id = id;
        _text = text;
    }

    public NotEmptyGuid WhatId() => _id;

    public NotEmptyString WhatText() => _text;
}
