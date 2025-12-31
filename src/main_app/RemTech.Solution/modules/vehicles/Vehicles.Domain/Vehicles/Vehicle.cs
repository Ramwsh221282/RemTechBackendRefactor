using Vehicles.Domain.Brands;
using Vehicles.Domain.Categories;
using Vehicles.Domain.Locations;
using Vehicles.Domain.Models;

namespace Vehicles.Domain.Vehicles;

public sealed class Vehicle(
    VehicleId id,
    BrandId brand,
    CategoryId category,
    LocationId location,
    ModelId model,
    VehicleSource source, 
    VehiclePriceInformation price, 
    VehicleTextInformation text, 
    VehiclePhotosGallery photos)
{
    public Vehicle(
        VehicleId id,
        Brand brand,
        Category category,
        Location location,
        Model model,
        VehicleSource source, 
        VehiclePriceInformation price, 
        VehicleTextInformation text, 
        VehiclePhotosGallery photos) 
        : this(id, brand.Id, category.Id, location.Id, model.Id, source, price, text, photos) { }
    
    public VehicleId Id { get; } = id;
    public BrandId BrandId { get; } = brand;
    public CategoryId CategoryId { get; } = category;
    public LocationId LocationId { get; } = location;
    public ModelId ModelId { get; } = model;
    public VehicleSource Source { get; } = source;
    public VehiclePriceInformation Price { get; } = price;
    public VehicleTextInformation Text { get; } = text;
    public VehiclePhotosGallery Photos { get; } = photos;
    public IReadOnlyList<VehicleCharacteristic> Characteristics { get; private set; } = [];

    public void AddCharacteristics(IEnumerable<VehicleCharacteristicToAdd> characteristics)
    {
        var comparer = new VehicleCharacteristicByNameComparer();
        HashSet<VehicleCharacteristic> result = new HashSet<VehicleCharacteristic>(comparer);
        foreach (VehicleCharacteristicToAdd characteristic in characteristics)
            result.Add(new VehicleCharacteristic(this, characteristic.Characteristic, characteristic.Value));
        Characteristics = result.ToList();
    }
}