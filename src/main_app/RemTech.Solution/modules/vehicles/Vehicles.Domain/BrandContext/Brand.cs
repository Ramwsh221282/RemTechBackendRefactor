using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.Infrastructure.DataSource;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Domain.BrandContext;

public sealed class Brand
{
    public BrandId Id { get; }
    public BrandName Name { get; private set; } = null!;
    public BrandRating Rating { get; private set; }
    public BrandOwnedVehiclesCount VehiclesCount { get; private set; }

    private Brand()
    {
        // ef core
    }

    private Brand(
        BrandId id,
        BrandName name,
        BrandRating rating,
        BrandOwnedVehiclesCount vehiclesCount
    )
    {
        Id = id;
        Name = name;
        Rating = rating;
        VehiclesCount = vehiclesCount;
    }

    public static async Task<Brand> Create(
        BrandName name,
        IBrandsDataSource brands,
        CancellationToken ct = default
    )
    {
        BrandId id = new();
        BrandRating rating = new();
        BrandOwnedVehiclesCount vehiclesCount = new();
        UniqueBrand unique = await brands.GetUniqueBrand(name, ct);
        return Create(id, name, rating, vehiclesCount, unique);
    }

    public static Result<Brand> Create(
        BrandId id,
        BrandName name,
        BrandRating rating,
        BrandOwnedVehiclesCount vehiclesCount,
        UniqueBrand unique
    )
    {
        Brand brand = new Brand(id, name, rating, vehiclesCount);
        return unique.ApproveUniqueness(brand);
    }
}
