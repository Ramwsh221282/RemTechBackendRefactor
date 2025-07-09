using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Decorators;

public sealed class ValidatingCharacteristicBakery : ICharacteristicBakery
{
    private readonly ICharacteristicBakery _origin;

    public ValidatingCharacteristicBakery(ICharacteristicBakery origin)
    {
        _origin = origin;
    }

    public Status<ICharacteristic> Baked(ICharacteristicReceipt receipt)
    {
        NotEmptyString name = receipt.WhatName();
        NotEmptyGuid id = receipt.WhatId();
        if (!name)
            return new ValidationError<ICharacteristic>("Некорректное название характеристики.");
        if (!id)
            return new ValidationError<ICharacteristic>("Некорректный ID характеристики.");
        return _origin.Baked(receipt);
    }
}
