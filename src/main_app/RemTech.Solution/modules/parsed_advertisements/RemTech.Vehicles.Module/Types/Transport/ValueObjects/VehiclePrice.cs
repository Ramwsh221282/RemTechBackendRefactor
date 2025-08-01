﻿using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Prices;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects;

public sealed class VehiclePrice
{
    private readonly ItemPrice _price;

    public VehiclePrice(ItemPrice price)
    {
        _price = price;
    }

    public ItemPrice Read() => _price;
}
