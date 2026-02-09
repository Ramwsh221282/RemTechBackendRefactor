using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Значение характеристики транспортного средства.
/// </summary>
public sealed record VehicleCharacteristicValue
{
	private VehicleCharacteristicValue(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение характеристики транспортного средства.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт значение характеристики транспортного средства.
	/// </summary>
	/// <param name="value">Значение характеристики транспортного средства.</param>
	/// <returns>Результат создания значения характеристики транспортного средства.</returns>
	public static Result<VehicleCharacteristicValue> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleCharacteristicValue>)Error.Validation("Значение характеристики не может быть пустым.")
			: (Result<VehicleCharacteristicValue>)new VehicleCharacteristicValue(value);
	}
}
