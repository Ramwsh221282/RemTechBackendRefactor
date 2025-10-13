using Vehicles.Domain.BrandContext;
using Vehicles.Domain.BrandContext.ValueObjects;
using Vehicles.Domain.CategoryContext;
using Vehicles.Domain.CategoryContext.ValueObjects;
using Vehicles.Domain.LocationContext;
using Vehicles.Domain.LocationContext.ValueObjects;
using Vehicles.Domain.ModelContext;
using Vehicles.Domain.ModelContext.ValueObjects;
using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.Domain.VehicleContext;

public sealed class Vehicle
{
    public required VehicleId Id { get; set; }
    public required Category Category { get; set; }
    public required CategoryId CategoryId { get; set; }
    public required Brand Brand { get; set; } = null!;
    public required BrandId BrandId { get; set; }
    public required Location Location { get; set; } = null!;
    public required LocationId LocationId { get; set; }
    public required VehicleModel Model { get; set; } = null!;
    public required VehicleModelId ModelId { get; set; }
    public required VehicleDescription Description { get; set; }
    public required VehiclePrice Price { get; set; }
    public required VehicleCharacteristicsCollection Characteristics { get; set; }
    public required VehiclePhotosCollection Photos { get; set; } = null!;
}
