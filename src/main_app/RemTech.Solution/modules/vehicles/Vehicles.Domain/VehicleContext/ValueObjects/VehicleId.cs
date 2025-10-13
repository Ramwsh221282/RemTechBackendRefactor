using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public readonly record struct VehicleId
{
    public Guid Value { get; }

    public VehicleId() => Value = Guid.NewGuid();

    private VehicleId(Guid value) => Value = value;

    public static Result<VehicleId> Create(Guid value) =>
        value == Guid.Empty ? new VehicleIdEmptyError() : new VehicleId(value);

    public static Result<VehicleId> Create(Guid? value) =>
        value == null ? new VehicleIdEmptyError() : new VehicleId(value.Value);
}
