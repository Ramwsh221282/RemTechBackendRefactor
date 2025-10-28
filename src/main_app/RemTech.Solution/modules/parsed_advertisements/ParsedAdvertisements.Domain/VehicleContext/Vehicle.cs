using ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

namespace ParsedAdvertisements.Domain.VehicleContext;

public sealed class Vehicle
{
    public Vehicle(
        VehicleIdentitySpecification identity,
        VehiclePriceSpecification price,
        VehicleSourceSpecification source,
        VehicleCharacteristicsList characteristics,
        VehiclePhotosList photos,
        VehicleLocationPath path
    )
    {
        Identity = identity;
        Price = price;
        Source = source;
        Characteristics = characteristics;
        Path = path;
        Photos = photos;
    }

    public VehicleIdentitySpecification Identity { get; }
    public VehiclePriceSpecification Price { get; }
    public VehicleSourceSpecification Source { get; }
    public VehicleCharacteristicsList Characteristics { get; }
    public VehicleLocationPath Path { get; }
    public VehiclePhotosList Photos { get; }
}