using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Bakery.Receipts.Decorators;

public sealed class FormattingVehicleBrandReceipt : IVehicleBrandReceipt
{
    private readonly IVehicleBrandReceipt _origin;
    private bool _formatted;
    private NotEmptyString _brandName;

    public FormattingVehicleBrandReceipt(IVehicleBrandReceipt receipt)
    {
        _origin = receipt;
        _brandName = receipt.WhatName();
    }

    public NotEmptyString WhatName()
    {
        if (_formatted)
            return _brandName;
        string text = new Text(
            new CapitalizedFirstLetterText(
                new TextWithoutPunctuation(new TrimmedText(new RawText(_brandName)))
            )
        ).Read();
        _brandName = new NotEmptyString(text);
        _formatted = true;
        return _brandName;
    }

    public NotEmptyGuid WhatId()
    {
        return _origin.WhatId();
    }

    public Status<IVehicleBrand> Baked(IVehicleBrandBakery bakery)
    {
        return bakery.Bake(this);
    }
}
