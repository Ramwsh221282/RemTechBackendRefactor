using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery.Receipts;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Bakery;

public sealed class CharacteristicBakery : ICharacteristicBakery
{
    public Status<ICharacteristic> Baked(ICharacteristicReceipt receipt) =>
        new Characteristic(
            new CharacteristicIdentity(
                new CharacteristicId(receipt.WhatId()),
                new CharacteristicText(receipt.WhatName())
            )
        );
}
