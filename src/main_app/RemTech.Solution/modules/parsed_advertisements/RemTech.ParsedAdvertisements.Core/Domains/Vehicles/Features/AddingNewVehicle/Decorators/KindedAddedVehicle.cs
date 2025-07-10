using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class KindedAddedVehicle(IVehicleKinds kinds, string? kindName, IAddedVehicle origin)
    : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add)
    {
        Status<VehicleKindEnvelope> kind = kinds.Add(kindName);
        return kind.IsFailure
            ? kind.Error
            : origin.Added(new AddVehicle(new KindedVehicle(kind.Value, add.WhatVehicle())));
    }
}
