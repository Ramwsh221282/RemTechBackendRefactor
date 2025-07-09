using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Decorators;

public sealed class CachingCharacteristicBakery : ICharacteristicBakery
{
    private readonly ICharacteristicBakery _origin;
    private MaybeBag<Status<ICharacteristic>> _bag;

    public CachingCharacteristicBakery(ICharacteristicBakery origin)
    {
        _origin = origin;
        _bag = new MaybeBag<Status<ICharacteristic>>();
    }

    public Status<ICharacteristic> Baked(ICharacteristicReceipt receipt)
    {
        if (_bag.Any())
            return _bag.Take();
        _bag = _bag.Put(_origin.Baked(receipt));
        return _bag.Take();
    }
}
