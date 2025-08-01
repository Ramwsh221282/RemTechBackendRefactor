﻿using RemTech.Vehicles.Module.Types.Brands.ValueObjects;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Models.Decorators.Logic;

public class BrandedVehicleModel
{
    private readonly VehicleBrandIdentity _brandIdentity;
    private readonly VehicleModelIdentity _modelIdentity;

    public BrandedVehicleModel()
    {
        _brandIdentity = new VehicleBrandIdentity();
        _modelIdentity = new VehicleModelIdentity();
    }

    public BrandedVehicleModel(BrandedVehicleModel origin)
    {
        _brandIdentity = origin._brandIdentity;
        _modelIdentity = origin._modelIdentity;
    }

    public BrandedVehicleModel(BrandedVehicleModel origin, VehicleBrandIdentity brandIdentity)
        : this(origin) => _brandIdentity = brandIdentity;

    public BrandedVehicleModel(BrandedVehicleModel origin, VehicleModelIdentity modelIdentity)
        : this(origin) => modelIdentity = modelIdentity;
}
