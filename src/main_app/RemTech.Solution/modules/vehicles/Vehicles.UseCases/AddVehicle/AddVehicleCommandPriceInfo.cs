using Vehicles.Domain.VehicleContext.ValueObjects;

namespace Vehicles.UseCases.AddVehicle;

public sealed record AddVehicleCommandPriceInfo(long Value, bool IsNds)
{
    public VehiclePrice ProvideVehiclePrice() => new(Value, IsNds);
}
