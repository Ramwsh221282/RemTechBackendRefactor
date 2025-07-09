using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Decorators;

public sealed class CachingVehicleGeoBakery : IVehicleGeoBakery
{
    private readonly IVehicleGeoBakery _bakery;
    private MaybeBag<Status<IGeoLocation>> _bag;

    public CachingVehicleGeoBakery(IVehicleGeoBakery bakery)
    {
        _bakery = bakery;
        _bag = new MaybeBag<Status<IGeoLocation>>();
    }

    public Status<IGeoLocation> Baked(IVehicleGeoReceipt receipt)
    {
        if (_bag.Any())
            return _bag.Take();
        _bag.Put(_bakery.Baked(receipt));
        return _bag.Take();
    }
}
