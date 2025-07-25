using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries;

public sealed class PriceExtraInspector
{
    private readonly NotEmptyString _extra;
    private readonly IItemPrice _price;

    public PriceExtraInspector(IItemPriceReceipt receipt, IItemPrice price)
    {
        _extra = receipt.AnyExtra();
        _price = price;
    }

    public PriceExtraInspector(NotEmptyString extra, IItemPrice price)
    {
        _extra = extra;
        _price = price;
    }

    public ItemPrice Inspected() =>
        ((string)_extra).Contains("НДС", StringComparison.OrdinalIgnoreCase)
            ? new ItemPrice(new ItemPriceWithNds(_price.Value()))
            : new ItemPrice(new ItemPriceWithoutNds(new PriceValue(_price.Value())));
}
