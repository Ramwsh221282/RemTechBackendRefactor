using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleTextInformation
{
	private VehicleTextInformation(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<VehicleTextInformation> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleTextInformation>)Error.Validation("Текстовая информация о технике не может быть пустой.")
			: (Result<VehicleTextInformation>)new VehicleTextInformation(value);
	}
}
