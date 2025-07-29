using RemTech.ParsedAdvertisements.Core.Types.Models.Decorators.Logic;
using RemTech.ParsedAdvertisements.Core.Types.Transport;

namespace RemTech.ParsedAdvertisements.Core.Types.Brands.Decorators.Logic;

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
