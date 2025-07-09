using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;

public interface ICharacteristicReceipt
{
    NotEmptyGuid WhatId();
    NotEmptyString WhatName();
}
