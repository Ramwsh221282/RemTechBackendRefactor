using RemTech.Vehicles.Module.Types.Models.Decorators.Logic;
using RemTech.Vehicles.Module.Types.Transport;

namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Logic;

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
