using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;

public sealed class ContainingPriceReceipt : IItemPriceReceipt
{
    private readonly PriceValue _value;
    private readonly NotEmptyString _priceExtra;

    public ContainingPriceReceipt(PriceValue value, NotEmptyString priceExtra)
    {
        _value = value;
        _priceExtra = priceExtra;
    }

    public ContainingPriceReceipt(PositiveLong value, NotEmptyString priceExtra)
        : this(new PriceValue(value), priceExtra) { }

    public PriceValue WhatValue() => _value;

    public NotEmptyString AnyExtra() => _priceExtra;
}
