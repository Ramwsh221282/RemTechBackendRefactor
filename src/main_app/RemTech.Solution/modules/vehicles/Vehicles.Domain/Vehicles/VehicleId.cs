using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleId
{
    public Guid Value { get; }

    private VehicleId(Guid value)
    {
        Value = value;
    }

    public static Result<VehicleId> Create(Guid value)
    {
        return value == Guid.Empty ? (Result<VehicleId>)Error.Validation("Идентификатор техники не может быть пустым.") : (Result<VehicleId>)new VehicleId(value);
    }
}
