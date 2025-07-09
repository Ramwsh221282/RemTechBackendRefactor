using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts.Decorators;

public sealed class FormattedCharacteristicReceipt : ICharacteristicReceipt
{
    private readonly ICharacteristicReceipt _receipt;
    private NotEmptyString _name;
    private bool _formatted;

    public FormattedCharacteristicReceipt(ICharacteristicReceipt receipt)
    {
        _receipt = receipt;
        _name = receipt.WhatName();
    }

    public NotEmptyGuid WhatId()
    {
        return _receipt.WhatId();
    }

    public NotEmptyString WhatName()
    {
        if (_formatted)
            return _name;
        _name = new NotEmptyString(
            new Text(
                new CapitalizedFirstLetterText(
                    new TrimmedText(new TextWithoutPunctuation(new RawText(_name)))
                )
            ).Read()
        );
        _formatted = true;
        return _name;
    }
}
