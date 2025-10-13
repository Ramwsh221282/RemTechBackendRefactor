using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext.Infrastructure.DataSource;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext;

public sealed class Vehicle
{
    public VehicleId Id { get; }
    public Category Category { get; } = null!;
    public CategoryId CategoryId { get; }
    public Brand Brand { get; } = null!;
    public BrandId BrandId { get; }
    public Location Location { get; } = null!;
    public LocationId LocationId { get; } = null!;
    public VehicleModel Model { get; } = null!;
    public VehicleModelId ModelId { get; }
    public VehicleDescription Description { get; } = null!;
    public VehiclePrice Price { get; }
    public VehicleCharacteristicsCollection Characteristics { get; } = null!;
    public VehiclePhotosCollection Photos { get; } = null!;

    private Vehicle()
    {
        // ef core
    }

    public Vehicle(
        Category category,
        Brand brand,
        VehicleModel model,
        Location location,
        VehicleDescription description,
        VehiclePrice price,
        VehicleCharacteristicsCollection characteristics,
        VehiclePhotosCollection photos,
        VehicleId? id = null
    )
    {
        Category = category;
        CategoryId = category.Id;
        Brand = brand;
        BrandId = brand.Id;
        Model = model;
        ModelId = model.Id;
        Location = location;
        LocationId = location.Id;
        Description = description;
        Price = price;
        Characteristics = characteristics;
        Photos = photos;
        Id = id ?? new VehicleId();
    }

    public static async Task<Vehicle> Add(
        IVehiclesDataSource dataSource,
        Category category,
        Brand brand,
        VehicleModel model,
        Location location,
        VehicleDescription description,
        VehiclePrice price,
        VehicleCharacteristicsCollection characteristics,
        VehiclePhotosCollection photos,
        VehicleId? id = null,
        CancellationToken ct = default
    )
    {
        Vehicle vehicle = new Vehicle(
            category,
            brand,
            model,
            location,
            description,
            price,
            characteristics,
            photos,
            id
        );
        await dataSource.Add(vehicle, ct);
        return vehicle;
    }
}
