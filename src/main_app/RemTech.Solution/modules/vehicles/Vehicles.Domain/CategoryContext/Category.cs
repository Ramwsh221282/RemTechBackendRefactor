using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Domain.CategoryContext;

public sealed class Category
{
    public required CategoryId Id { get; set; }
    public required CategoryName Name { get; set; }
    public required CategoryRating Rating { get; set; }
    public required CategoryVehiclesCount VehiclesCount { get; set; }
}
