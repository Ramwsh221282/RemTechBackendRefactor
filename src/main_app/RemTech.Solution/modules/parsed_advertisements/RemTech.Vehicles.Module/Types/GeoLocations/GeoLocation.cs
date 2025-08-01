﻿using RemTech.Vehicles.Module.Types.GeoLocations.ValueObjects;

namespace RemTech.Vehicles.Module.Types.GeoLocations;

public class GeoLocation
{
    protected virtual GeoLocationIdentity Identity { get; }

    public GeoLocation(GeoLocationIdentity identity) => Identity = identity;

    public GeoLocation(GeoLocation origin) => Identity = origin.Identity;

    public Guid Id() => Identity.ReadId();

    public string Name() => Identity.ReadText();

    public string Kind() => Identity.Kind();
}
