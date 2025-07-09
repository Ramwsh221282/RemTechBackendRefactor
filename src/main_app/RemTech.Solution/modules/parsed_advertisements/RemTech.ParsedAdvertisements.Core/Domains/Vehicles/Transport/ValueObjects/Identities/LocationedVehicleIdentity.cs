using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Identities;

public sealed class LocationedVehicleIdentity
{
    private readonly GeoLocationIdentity _location;
    private readonly OriginVehicleIdentity _origin;

    public LocationedVehicleIdentity(GeoLocationIdentity location, OriginVehicleIdentity origin)
    {
        _location = location;
        _origin = origin;
    }

    public LocationedVehicleIdentity(GeoLocation location, OriginVehicleIdentity origin)
        : this(location.Identify(), origin) { }

    public GeoLocationIdentity ReadLocation()
    {
        return _location;
    }

    public OriginVehicleIdentity WhatOrigin() => _origin;
}
