using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;

public interface IItemPriceReceipt
{
    public PriceValue WhatValue();
    public NotEmptyString AnyExtra();
}
