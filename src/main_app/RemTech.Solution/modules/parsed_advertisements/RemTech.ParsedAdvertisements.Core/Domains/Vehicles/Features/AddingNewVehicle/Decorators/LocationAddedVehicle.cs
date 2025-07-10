using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.Ports;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.AddingNewVehicle.Decorators;

public sealed class LocationAddedVehicle(
    IGeoLocations locations,
    string? locationName,
    IAddedVehicle origin
) : IAddedVehicle
{
    public Status<VehicleEnvelope> Added(AddVehicle add)
    {
        Status<GeoLocationEnvelope> location = locations.Add(locationName);
        return location.IsFailure
            ? location.Error
            : origin.Added(
                new AddVehicle(new LocationedVehicleEnvelope(location.Value, add.WhatVehicle()))
            );
    }
}
