using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext.Features.VehicleRegistration;

public sealed class MovementVehicleRegistrator : IVehicleRegistrator
{
    private readonly IVehicleRegistrator _registrator;

    public MovementVehicleRegistrator(IVehicleRegistrator registrator) =>
        _registrator = registrator;

    public async Task<Vehicle> RegisterVehicle(
        CategoryName categoryName,
        BrandName brandName,
        VehicleModelName modelName,
        LocationAddress address,
        VehicleDescription description,
        VehiclePrice price,
        VehicleCharacteristicsCollection characteristics,
        VehiclePhotosCollection photos,
        CancellationToken ct = default
    )
    {
        Vehicle vehicle = await _registrator.RegisterVehicle(
            categoryName,
            brandName,
            modelName,
            address,
            description,
            price,
            characteristics,
            photos,
            ct
        );
        vehicle.Brand.AddVehicle(vehicle);
        vehicle.Model.AddVehicle(vehicle);
        vehicle.Category.AddVehicle(vehicle);
        vehicle.Location.AddVehicle(vehicle);
        return vehicle;
    }
}
