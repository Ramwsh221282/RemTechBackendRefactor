using RemTech.UseCases.Shared.Cqrs;
using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.UseCases.AddVehicle;

public sealed record AddVehicleCommand(
    string CategoryName,
    string BrandName,
    string ModelName,
    IEnumerable<string> LocationParts,
    string Description,
    AddVehicleCommandPriceInfo Price,
    IEnumerable<AddVehicleCommandCharacteristic> Characteristics,
    IEnumerable<string> PhotoPaths
) : ICommand<Vehicle>
{
    public Vehicle ProvideVehicle()
    {
        Category category = ProvideCategory();
        Brand brand = ProvideBrand();
        Location location = ProvideLocation();
        VehicleModel model = ProvideVehicleModel();
        return new Vehicle()
        {
            Id = new VehicleId(Guid.NewGuid()),
            Brand = brand,
            BrandId = brand.Id,
            Category = category,
            CategoryId = category.Id,
            Model = model,
            ModelId = model.Id,
            Location = location,
            LocationId = location.Id,
            Price = Price.ProvideVehiclePrice(),
            Description = new VehicleDescription(Description),
            Photos = new VehiclePhotosCollection([.. PhotoPaths.Select(p => new VehiclePhoto(p))]),
            Characteristics = new VehicleCharacteristicsCollection(
                [.. Characteristics.Select(c => c.ProvideCharacteristic())]
            ),
        };
    }

    private Category ProvideCategory() =>
        new()
        {
            Name = new CategoryName(CategoryName),
            Id = new CategoryId(Guid.NewGuid()),
            Rating = new CategoryRating(),
            VehiclesCount = new CategoryVehiclesCount(),
        };

    private Brand ProvideBrand() =>
        new()
        {
            Name = new BrandName(BrandName),
            Rating = new BrandRating(),
            VehiclesCount = new BrandVehiclesCount(),
            Id = new BrandId(Guid.NewGuid()),
        };

    private VehicleModel ProvideVehicleModel() =>
        new()
        {
            Id = new VehicleModelId(Guid.NewGuid()),
            Name = new VehicleModelName(ModelName),
            Rating = new VehicleModelRating(),
            VehiclesCount = new VehicleModelVehicleCount(),
        };

    private Location ProvideLocation() =>
        new()
        {
            Id = new LocationId(Guid.NewGuid()),
            Rating = new LocationRating(),
            VehicleCount = new LocationVehiclesCount(),
            Address = new LocationAddress(
                LocationParts.Select(p => new LocationAddressPart(p)).ToList()
            ),
        };
}
