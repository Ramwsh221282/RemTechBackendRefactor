using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle;

public interface IAddVehicle
{
    Status<VehicleEnvelope> WhatVehicle();
}

public sealed class AddVehicle : IAddVehicle
{
    private readonly VehicleEnvelope _vehicle;

    public AddVehicle(VehicleEnvelope vehicle) => _vehicle = vehicle;

    public Status<VehicleEnvelope> WhatVehicle() => _vehicle;
}

public sealed class AddedVehicle(IVehicles vehicles)
{
    public Status<VehicleEnvelope> Added(IAddVehicle vehicle) =>
        vehicles.Add(vehicle.WhatVehicle());
}
