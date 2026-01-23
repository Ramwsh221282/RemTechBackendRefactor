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
		if (value == Guid.Empty)
			return Error.Validation("Идентификатор техники не может быть пустым.");
		return new VehicleId(value);
	}
}
