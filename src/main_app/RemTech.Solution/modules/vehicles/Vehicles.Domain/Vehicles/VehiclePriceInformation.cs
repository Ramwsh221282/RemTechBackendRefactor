using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehiclePriceInformation
{
    public long Value { get; }
    public bool IsNds { get; }

    private VehiclePriceInformation(long value, bool isNds)
    {
        Value = value;
        IsNds = isNds;
    }

    public static Result<VehiclePriceInformation> Create(long value, bool isNds)
    {
        return value <= 0
            ? (Result<VehiclePriceInformation>)Error.Validation("Цена техники не может быть меньше или равной нулю.")
            : (Result<VehiclePriceInformation>)new VehiclePriceInformation(value, isNds);
    }
}
