using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Texts;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Bakery.Receipts.Decorators;

public sealed class FormattedVehicleGeoReceipt : IVehicleGeoReceipt
{
    private readonly IVehicleGeoReceipt _receipt;
    private bool _isFormatted;
    private NotEmptyString _formatted;

    public FormattedVehicleGeoReceipt(IVehicleGeoReceipt receipt)
    {
        _receipt = receipt;
        _formatted = receipt.WhatText();
    }

    public NotEmptyGuid WhatId()
    {
        return _receipt.WhatId();
    }

    public NotEmptyString WhatText()
    {
        if (_isFormatted)
            return _formatted;
        _formatted = new NotEmptyString(
            new Text(
                new CapitalizedFirstLetterText(
                    new TrimmedText(new TextWithoutPunctuation(new RawText(_formatted)))
                )
            ).Read()
        );
        _isFormatted = true;
        return _formatted;
    }
}
