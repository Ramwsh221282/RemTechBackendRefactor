using RemTech.Vehicles.Module.Types.Transport;

namespace RemTech.Vehicles.Module.Types.Models.Decorators.Logic;

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
