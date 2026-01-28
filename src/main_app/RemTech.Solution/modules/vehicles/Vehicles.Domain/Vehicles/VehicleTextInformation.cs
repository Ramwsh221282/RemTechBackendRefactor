using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Текстовая информация о транспортном средстве.
/// </summary>
public sealed record VehicleTextInformation
{
	private VehicleTextInformation(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Текстовая информация о транспортном средстве.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт текстовую информацию о транспортном средстве.
	/// </summary>
	/// <param name="value">Текстовая информация о транспортном средстве.</param>
	/// <returns>Результат создания текстовой информации о транспортном средстве.</returns>
	public static Result<VehicleTextInformation> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleTextInformation>)Error.Validation("Текстовая информация о технике не может быть пустой.")
			: (Result<VehicleTextInformation>)new VehicleTextInformation(value);
	}
}
