using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class CharacterizedAddedVehicle(
    IEnumerable<(string? name, string? value)> ctx,
    ICharacteristics characteristics,
    IAddedVehicle origin
) : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add)
    {
        List<VehicleCharacteristic> created = [];
        foreach ((string? name, string? value) entry in ctx)
        {
            Status<CharacteristicEnvelope> addedCtx = characteristics.Add(entry.name);
            if (addedCtx.IsSuccess)
                created.Add(
                    new VehicleCharacteristic(
                        addedCtx.Value,
                        new VehicleCharacteristicValue(entry.value)
                    )
                );
        }

        return origin.Added(new AddVehicle(new CharacterizedVehicle(created, add.WhatVehicle())));
    }
}
