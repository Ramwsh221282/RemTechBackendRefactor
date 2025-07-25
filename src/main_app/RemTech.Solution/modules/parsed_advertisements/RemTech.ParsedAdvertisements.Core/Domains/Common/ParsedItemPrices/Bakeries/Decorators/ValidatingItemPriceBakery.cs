using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Decorators;

public sealed class ValidatingItemPriceBakery(IItemPriceBakery origin) : IItemPriceBakery
{
    public Status<IItemPrice> Baked(IItemPriceReceipt receipt) =>
        !receipt.WhatValue()
            ? new ValidationError<IItemPrice>("Цена должна быть положительным числом.")
            : origin.Baked(receipt);
}
