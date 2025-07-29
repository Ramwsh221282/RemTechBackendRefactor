using RemTech.ParsedAdvertisements.Core.Types.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Logic;

public sealed class UnknownVehicleBrand : VehicleBrand
{
    public UnknownVehicleBrand()
        : base(new VehicleBrandIdentity()) { }
}
