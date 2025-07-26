using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Models.Decorators.Logic;

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