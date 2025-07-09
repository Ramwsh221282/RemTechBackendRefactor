using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

public interface IVehicle
{
    VehicleIdentity Identify();
    VehiclePrice WhatCost();
    VehicleText TextInformation();
    VehiclePhotos WatchPhotos();
    VehicleCharacteristics WhatCharacteristics();
}
