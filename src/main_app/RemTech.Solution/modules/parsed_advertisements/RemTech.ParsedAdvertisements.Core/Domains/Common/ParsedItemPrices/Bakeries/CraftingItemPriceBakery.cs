using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries;

public sealed class CraftingItemPriceBakery : IItemPriceBakery
{
    public Status<IItemPrice> Baked(IItemPriceReceipt receipt) =>
        new ItemPrice(new ItemPriceWithoutNds(receipt.WhatValue()));
}
