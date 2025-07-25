﻿using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;

public class VehicleModel
{
    protected virtual VehicleModelIdentity Identity { get; }
    protected virtual VehicleModelName Name { get; }

    public VehicleModel(VehicleModelIdentity identity, VehicleModelName name)
    {
        Identity = identity;
        Name = name;
    }

    public VehicleModel()
    {
        Identity = new VehicleModelIdentity();
        Name = new VehicleModelName();
    }

    public VehicleModel(VehicleModel origin)
    {
        Identity = origin.Identity;
        Name = origin.Name;
    }

    public Guid Id() => Identity;
    public string NameString() => Name;
}