using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public sealed class Vehicle : IVehicle
{
    private readonly VehicleIdentity _identity;
    private readonly VehiclePrice _price;
    private readonly VehicleText _text;
    private readonly VehiclePhotos _photos;
    private readonly VehicleCharacteristics _characteristics;

    public Vehicle(
        VehicleIdentity identity,
        VehiclePrice price,
        VehicleText text,
        VehiclePhotos photos,
        VehicleCharacteristics characteristics
    )
    {
        _identity = identity;
        _price = price;
        _text = text;
        _photos = photos;
        _characteristics = characteristics;
    }

    public VehicleIdentity Identify()
    {
        return _identity;
    }

    public VehiclePrice WhatCost()
    {
        return _price;
    }

    public VehicleText TextInformation()
    {
        return _text;
    }

    public VehiclePhotos WatchPhotos()
    {
        return _photos;
    }

    public VehicleCharacteristics WhatCharacteristics()
    {
        return _characteristics;
    }
}
