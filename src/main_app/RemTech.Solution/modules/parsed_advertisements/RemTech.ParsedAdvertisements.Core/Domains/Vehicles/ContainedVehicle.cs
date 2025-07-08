using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles;

public sealed class ContainedVehicle
{
    private readonly VehicleOfKind _kind;
    private readonly VehicleOfBrand _brand;
    private readonly VehicleOfGeo _geo;
    private readonly VehicleCharacteristic[] _characteristics;

    public ContainedVehicle(
        ParsedTransport transport,
        ParsedVehicleKind kind,
        ParsedVehicleBrand brand,
        ParsedGeoLocation geo,
        ParsedVehicleCharacteristic[] characteristics
    )
    {
        _kind = transport.Specify(kind);
        _brand = transport.Specify(brand);
        _geo = transport.Specify(geo);
        foreach (ParsedVehicleCharacteristic characteristic in characteristics)
            transport.WithCharacteristic(characteristic);
        _characteristics = transport.WhatCharacteristics();
    }

    public ContainedVehicle(VehicleContainer container)
    {
        ContainedVehicle contained = container.Contain();
        _kind = contained._kind;
        _brand = contained._brand;
        _geo = contained._geo;
        _characteristics = contained._characteristics;
    }
}
