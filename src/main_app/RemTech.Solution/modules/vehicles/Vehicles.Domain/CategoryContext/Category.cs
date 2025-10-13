using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.Infrastructure.DataSource;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.VehicleContext;

namespace Vehicles.Domain.CategoryContext;

public sealed class Category
{
    public CategoryId Id { get; }
    public CategoryName Name { get; } = null!;
    public CategoryRating Rating { get; }
    public CategoryOwnedVehiclesCount OwnedVehiclesCount { get; private set; }

    private Category()
    {
        // ef core
    }

    private Category(
        CategoryId id,
        CategoryName name,
        CategoryRating rating,
        CategoryOwnedVehiclesCount ownedVehiclesCount
    )
    {
        Id = id;
        Name = name;
        Rating = rating;
        OwnedVehiclesCount = ownedVehiclesCount;
    }

    public static async Task<Result<Category>> Create(
        CategoryName name,
        ICategoryDataSource dataSource,
        CancellationToken ct = default
    )
    {
        UniqueCategory unique = await dataSource.GetUnique(name, ct);
        CategoryId id = new();
        CategoryOwnedVehiclesCount vehiclesCount = new();
        CategoryRating rating = new();
        return Create(id, name, rating, vehiclesCount, unique);
    }

    public static Result<Category> Create(
        CategoryId id,
        CategoryName name,
        CategoryRating rating,
        CategoryOwnedVehiclesCount ownedVehiclesCount,
        UniqueCategory unique
    )
    {
        Category category = new Category(id, name, rating, ownedVehiclesCount);
        return unique.ApproveUniqueness(category);
    }

    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle.CategoryId != Id)
            return;
        OwnedVehiclesCount = OwnedVehiclesCount.Increase();
    }
}
