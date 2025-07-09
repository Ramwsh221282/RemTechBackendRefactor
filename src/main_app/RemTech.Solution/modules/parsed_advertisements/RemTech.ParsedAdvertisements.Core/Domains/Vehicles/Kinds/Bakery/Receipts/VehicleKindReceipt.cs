using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;

public sealed class VehicleKindReceipt : IVehicleKindReceipt
{
    private readonly NotEmptyGuid _id;
    private readonly NotEmptyString _name;

    public NotEmptyGuid WhatId() => _id;

    public NotEmptyString WhatName() => _name;
}
