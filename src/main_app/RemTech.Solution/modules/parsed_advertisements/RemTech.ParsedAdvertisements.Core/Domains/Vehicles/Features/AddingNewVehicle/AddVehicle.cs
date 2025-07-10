using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle;

public sealed class AddVehicle
{
    private readonly VehicleEnvelope _vehicle;

    public AddVehicle(VehicleEnvelope vehicle) => _vehicle = vehicle;

    public VehicleEnvelope WhatVehicle() => _vehicle;
}