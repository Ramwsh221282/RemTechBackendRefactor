using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.Infrastructure.DataSource;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.Infrastructure;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext.Features.VehicleRegistration;

public sealed class CreationalVehicleRegistrator : IVehicleRegistrator
{
    private readonly ICategoryDataSource _categories;
    private readonly IBrandsDataSource _brands;
    private readonly ILocationsDataSource _locations;
    private readonly IVehicleModelsDataSource _models;
    private readonly IVehiclesDataSource _vehicles;

    public CreationalVehicleRegistrator(
        ICategoryDataSource categories,
        IBrandsDataSource brands,
        ILocationsDataSource locations,
        IVehicleModelsDataSource models,
        IVehiclesDataSource vehicles
    )
    {
        _categories = categories;
        _brands = brands;
        _locations = locations;
        _models = models;
        _vehicles = vehicles;
    }

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
        Category category = await _categories.GetOrSave(categoryName, ct);
        Brand brand = await _brands.GetOrSave(brandName, ct);
        VehicleModel model = await _models.GetOrSave(modelName, ct);
        Location location = await _locations.GetOrSave(address, ct);
        return await Vehicle.Create(
            _vehicles,
            category,
            brand,
            model,
            location,
            description,
            price,
            characteristics,
            photos,
            ct: ct
        );
    }
}
