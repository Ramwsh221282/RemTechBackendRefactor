using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class PhotographedAddedVehicle(IEnumerable<string?> photos, IAddedVehicle origin)
    : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add) =>
        origin.Added(
            new AddVehicle(
                new UniquePhotosVehicle(
                    new VehiclePhotos(
                        photos.Where(p => !string.IsNullOrEmpty(p)).Select(p => new VehiclePhoto(p))
                    ),
                    add.WhatVehicle()
                )
            )
        );
}
