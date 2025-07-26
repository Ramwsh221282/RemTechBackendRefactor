using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.Decorators.Logic;

public sealed class BrandingVehicleBrand(VehicleBrand brand) : VehicleBrand(brand)
{
    public BrandedVehicleModel BrandModel(BrandedVehicleModel model)
    {
        return new BrandedVehicleModel(model, Identity);
    }

    public Vehicle BrandVehicle(Vehicle vehicle)
    {
        return new Vehicle(vehicle, this);
    }
}