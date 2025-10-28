using ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

namespace ParsedAdvertisements.Domain.VehicleContext;

public sealed class Vehicle
{
    private Vehicle(
        VehicleIdentitySpecification identity,
        VehiclePriceSpecification price,
        VehicleSourceSpecification source,
        VehicleCharacteristicsList characteristics
    )
    {
        Identity = identity;
        Price = price;
        Source = source;
        Characteristics = characteristics;
    }

    public VehicleIdentitySpecification Identity { get; }
    public VehiclePriceSpecification Price { get; }
    public VehicleSourceSpecification Source { get; }
    public VehicleCharacteristicsList Characteristics { get; }
}
