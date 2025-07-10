using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class PricedAddedVehicle(long? price, string? extra, IAddedVehicle origin)
    : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add) =>
        origin.Added(new AddVehicle(new PricedVehicle(price, extra, add.WhatVehicle())));
}
