using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts.Decorators;

public sealed class FormattedVehicleKindReceipt : IVehicleKindReceipt
{
    private readonly IVehicleKindReceipt _origin;
    private NotEmptyString _name;
    private bool _formatted;

    public FormattedVehicleKindReceipt(IVehicleKindReceipt origin)
    {
        _origin = origin;
        _name = origin.WhatName();
    }

    public NotEmptyGuid WhatId()
    {
        return _origin.WhatId();
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
