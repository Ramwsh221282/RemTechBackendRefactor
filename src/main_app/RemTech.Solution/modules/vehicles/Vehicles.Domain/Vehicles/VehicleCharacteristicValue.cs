using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleCharacteristicValue
{
	private VehicleCharacteristicValue(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<VehicleCharacteristicValue> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleCharacteristicValue>)Error.Validation("Значение характеристики не может быть пустым.")
			: (Result<VehicleCharacteristicValue>)new VehicleCharacteristicValue(value);
	}
}
