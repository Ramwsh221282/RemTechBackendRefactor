using RemTech.ParsedAdvertisements.Core.Types.Transport;

namespace RemTech.ParsedAdvertisements.Core.Types.Models.Decorators.Logic;

public sealed class ModelingVehicleModel(VehicleModel origin) : VehicleModel(origin)
{
    public Vehicle ModeledVehicle(Vehicle vehicle)
    {
        return new Vehicle(vehicle, this);
    }

    public BrandedVehicleModel ModelBranded(BrandedVehicleModel branded)
    {
        return new BrandedVehicleModel(branded, Identity);
    }
}
