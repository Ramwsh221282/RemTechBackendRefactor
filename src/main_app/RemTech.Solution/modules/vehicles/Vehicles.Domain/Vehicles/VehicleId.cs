using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleId
{
	private VehicleId(Guid value)
	{
		Value = value;
	}

	public Guid Value { get; }

	public static Result<VehicleId> Create(Guid value)
	{
		return value == Guid.Empty
			? (Result<VehicleId>)Error.Validation("Идентификатор техники не может быть пустым.")
			: (Result<VehicleId>)new VehicleId(value);
	}
}
