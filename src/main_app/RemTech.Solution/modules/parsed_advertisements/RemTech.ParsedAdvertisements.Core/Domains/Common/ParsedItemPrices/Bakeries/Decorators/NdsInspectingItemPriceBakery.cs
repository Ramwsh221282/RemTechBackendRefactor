using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Decorators;

public sealed class NdsInspectingItemPriceBakery(IItemPriceBakery origin) : IItemPriceBakery
{
    public Status<IItemPrice> Baked(IItemPriceReceipt receipt) =>
        new PriceExtraInspector(receipt, origin.Baked(receipt).Value).Inspected();
}
