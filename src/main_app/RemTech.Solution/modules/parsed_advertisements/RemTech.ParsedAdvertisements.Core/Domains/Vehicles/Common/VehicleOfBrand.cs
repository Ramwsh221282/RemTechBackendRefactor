using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Common;

public sealed class VehicleOfBrand
{
    private readonly ParsedTransport _transport;
    private readonly ParsedVehicleBrand _brand;

    public VehicleOfBrand(ParsedTransport transport, ParsedVehicleBrand brand)
    {
        _transport = transport;
        _brand = brand;
    }

    public ParsedTransport WhatTransport() => _transport;

    public ParsedVehicleBrand WhatBrand() => _brand;

    public static implicit operator ParsedTransport(VehicleOfBrand vob)
    {
        return vob._transport;
    }

    public static implicit operator ParsedVehicleBrand(VehicleOfBrand vob)
    {
        return vob._brand;
    }
}
