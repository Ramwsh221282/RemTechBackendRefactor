using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Decorators;

public sealed class CachingVehicleKindBakery : IVehicleKindBakery
{
    private readonly IVehicleKindBakery _origin;
    private MaybeBag<Status<IVehicleKind>> _bag;

    public CachingVehicleKindBakery(IVehicleKindBakery origin)
    {
        _origin = origin;
        _bag = new MaybeBag<Status<IVehicleKind>>();
    }

    public Status<IVehicleKind> Baked(IVehicleKindReceipt receipt)
    {
        if (_bag.Any())
            return _bag.Take();
        _bag = _bag.Put(_origin.Baked(receipt));
        return _bag.Take();
    }
}
