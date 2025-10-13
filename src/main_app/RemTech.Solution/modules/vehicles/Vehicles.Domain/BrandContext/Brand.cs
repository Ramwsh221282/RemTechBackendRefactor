using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Domain.BrandContext;

public sealed class Brand
{
    public required BrandId Id { get; set; }
    public required BrandName Name { get; set; }
    public required BrandRating Rating { get; set; }
    public required BrandVehiclesCount VehiclesCount { get; set; }
}
