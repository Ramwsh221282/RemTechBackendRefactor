using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Bakery;

public sealed class VehicleKindBakery : IVehicleKindBakery
{
    public Status<IVehicleKind> Baked(IVehicleKindReceipt receipt) =>
        new VehicleKind(
            new VehicleKindIdentity(
                new VehicleKindId(receipt.WhatId()),
                new VehicleKindText(receipt.WhatName())
            )
        );
}
