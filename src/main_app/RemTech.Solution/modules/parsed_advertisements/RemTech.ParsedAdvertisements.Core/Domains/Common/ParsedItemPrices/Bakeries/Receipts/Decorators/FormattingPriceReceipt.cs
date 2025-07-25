using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts.Decorators;

public sealed class FormattingPriceReceipt(IItemPriceReceipt origin) : IItemPriceReceipt
{
    private NotEmptyString _extra = origin.AnyExtra();
    private bool _formatted;

    public PriceValue WhatValue() => origin.WhatValue();

    public NotEmptyString AnyExtra()
    {
        if (_formatted)
            return _extra;
        _extra = new NotEmptyString(
            new Text(new TrimmedText(new TextWithoutPunctuation(new RawText(_extra)))).Read()
        );
        _formatted = true;
        return _extra;
    }
}
