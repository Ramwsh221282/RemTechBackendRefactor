using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;

public sealed class CharacteristicReceipt : ICharacteristicReceipt
{
    private readonly NotEmptyGuid _id;
    private readonly NotEmptyString _name;

    public NotEmptyGuid WhatId() => _id;

    public NotEmptyString WhatName() => _name;
}
