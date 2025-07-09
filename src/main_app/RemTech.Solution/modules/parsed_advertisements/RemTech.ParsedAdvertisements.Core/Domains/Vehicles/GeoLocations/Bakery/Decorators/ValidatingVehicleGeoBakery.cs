using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Decorators;

public sealed class ValidatingVehicleGeoBakery : IVehicleGeoBakery
{
    private readonly IVehicleGeoBakery _bakery;

    public ValidatingVehicleGeoBakery(IVehicleGeoBakery bakery)
    {
        _bakery = bakery;
    }

    public Status<IGeoLocation> Baked(IVehicleGeoReceipt receipt)
    {
        NotEmptyGuid whatId = receipt.WhatId();
        NotEmptyString whatText = receipt.WhatText();
        if (!whatId)
            return Status<IGeoLocation>.Failure(new ValidationError("ID геолокации некоректный."));
        if (!whatText)
            return Status<IGeoLocation>.Failure(
                new ValidationError("Название геолокации некорректно.")
            );
        return _bakery.Baked(receipt);
    }
}
