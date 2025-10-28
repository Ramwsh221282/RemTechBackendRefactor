using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed record VehiclePriceSpecification
{
    public double Value { get; }
    public bool IsNds { get; }

    private VehiclePriceSpecification(double value, bool isNds)
    {
        Value = value;
        IsNds = isNds;
    }

    public static Status<VehiclePriceSpecification> Create(double value, bool isNds = false)
    {
        if (value < 0)
            return Error.Validation("Цена техники не может быть отрицательной.");
        return new VehiclePriceSpecification(value, isNds);
    }
}
