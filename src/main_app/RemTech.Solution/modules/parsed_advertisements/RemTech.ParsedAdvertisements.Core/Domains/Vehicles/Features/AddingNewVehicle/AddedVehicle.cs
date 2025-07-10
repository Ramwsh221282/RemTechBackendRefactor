using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle;

public sealed class AddedVehicle(IVehicles vehicles) : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle vehicle) => vehicles.Add(vehicle.WhatVehicle());
}
