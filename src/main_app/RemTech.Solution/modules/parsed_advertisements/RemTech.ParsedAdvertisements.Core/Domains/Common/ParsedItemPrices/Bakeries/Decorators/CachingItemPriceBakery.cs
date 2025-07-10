using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Decorators;

public sealed class CachingItemPriceBakery(IItemPriceBakery origin) : IItemPriceBakery
{
    private readonly MaybeBag<Status<IItemPrice>> _bag = new();

    public Status<IItemPrice> Baked(IItemPriceReceipt receipt) =>
        _bag.Any() ? _bag.Take() : _bag.Put(origin.Baked(receipt));
}
