using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery;

public sealed class VehicleGeoBakery : IVehicleGeoBakery
{
    public Status<IGeoLocation> Baked(IVehicleGeoReceipt receipt) =>
        new GeoLocation(
            new GeoLocationIdentity(
                new GeoLocationId(receipt.WhatId()),
                new GeolocationText(new NotEmptyString(receipt.WhatText()))
            )
        );
}
