using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Источник транспортного средства.
/// </summary>
public sealed record VehicleSource
{
	private VehicleSource(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Источник транспортного средства.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт источник транспортного средства.
	/// </summary>
	/// <param name="value">Источник транспортного средства.</param>
	/// <returns>Результат создания источника транспортного средства.</returns>
	public static Result<VehicleSource> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<VehicleSource>)Error.Validation("Источник техники не может быть пустым.")
			: (Result<VehicleSource>)new VehicleSource(value);
	}
}
