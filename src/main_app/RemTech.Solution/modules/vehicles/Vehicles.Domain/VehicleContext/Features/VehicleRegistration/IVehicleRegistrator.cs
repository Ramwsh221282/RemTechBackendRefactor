using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext.Features.VehicleRegistration;

public interface IVehicleRegistrator
{
    Task<Vehicle> RegisterVehicle(
        CategoryName categoryName,
        BrandName brandName,
        VehicleModelName modelName,
        LocationAddress address,
        VehicleDescription description,
        VehiclePrice price,
        VehicleCharacteristicsCollection characteristics,
        VehiclePhotosCollection photos,
        CancellationToken ct = default
    );
}
