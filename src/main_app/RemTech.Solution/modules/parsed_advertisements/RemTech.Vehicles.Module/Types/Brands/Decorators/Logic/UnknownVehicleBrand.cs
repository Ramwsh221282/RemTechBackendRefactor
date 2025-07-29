using RemTech.Vehicles.Module.Types.Brands.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Logic;

public sealed class UnknownVehicleBrand : VehicleBrand
{
    public UnknownVehicleBrand()
        : base(new VehicleBrandIdentity()) { }
}
