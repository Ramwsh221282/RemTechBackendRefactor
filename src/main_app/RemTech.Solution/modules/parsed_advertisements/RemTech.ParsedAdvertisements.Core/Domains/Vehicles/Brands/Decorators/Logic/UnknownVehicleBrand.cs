using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators.Logic;

public sealed class UnknownVehicleBrand : VehicleBrand
{
    public UnknownVehicleBrand() : base(new VehicleBrandIdentity())
    {
        
    }
}