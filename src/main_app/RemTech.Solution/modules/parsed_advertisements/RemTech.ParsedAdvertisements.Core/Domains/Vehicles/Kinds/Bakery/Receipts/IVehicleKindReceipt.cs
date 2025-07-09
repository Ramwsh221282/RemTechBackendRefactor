using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;

public interface IVehicleKindReceipt
{
    NotEmptyGuid WhatId();
    NotEmptyString WhatName();
}
