using RemTech.Result.Pattern;
using Vehicles.Domain.ModelContext.Errors;

namespace Vehicles.Domain.ModelContext.ValueObjects;

public readonly record struct VehicleModelId
{
    public Guid Value { get; }

    public VehicleModelId() => Value = Guid.NewGuid();

    private VehicleModelId(Guid value) => Value = value;

    public static Result<VehicleModelId> Create(Guid value) =>
        value == Guid.Empty ? new VehicleModelIdEmptyError() : new VehicleModelId(value);

    public static Result<VehicleModelId> Create(Guid? value) =>
        value == null ? new VehicleModelIdEmptyError() : Create(value.Value);
}
