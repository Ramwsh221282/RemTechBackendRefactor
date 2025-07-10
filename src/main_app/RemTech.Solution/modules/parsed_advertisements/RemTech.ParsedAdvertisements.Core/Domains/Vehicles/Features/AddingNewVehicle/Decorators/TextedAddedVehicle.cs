using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class TextedAddedVehicle(string? title, string? description, IAddedVehicle origin)
    : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add) =>
        origin.Added(
            new AddVehicle(new TextFormattedVehicle(title, description, add.WhatVehicle()))
        );
}
