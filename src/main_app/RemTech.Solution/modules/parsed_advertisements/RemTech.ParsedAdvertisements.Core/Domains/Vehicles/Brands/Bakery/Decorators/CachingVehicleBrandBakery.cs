using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Decorators;

public sealed class CachingVehicleBrandBakery : IVehicleBrandBakery
{
    private readonly IVehicleBrandBakery _origin;
    private MaybeBag<Status<IVehicleBrand>> _vehicleBrand;

    public CachingVehicleBrandBakery(IVehicleBrandBakery origin)
    {
        _origin = origin;
        _vehicleBrand = new MaybeBag<Status<IVehicleBrand>>();
    }

    public Status<IVehicleBrand> Bake(IVehicleBrandReceipt receipt)
    {
        if (_vehicleBrand.Any())
            return _vehicleBrand.Take();
        _vehicleBrand = _vehicleBrand.Put(_origin.Bake(receipt));
        return _vehicleBrand.Take();
    }
}
