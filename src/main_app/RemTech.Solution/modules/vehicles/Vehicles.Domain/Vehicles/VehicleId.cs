using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Идентификатор транспортного средства.
/// </summary>
public sealed record VehicleId
{
	private VehicleId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Уникальный идентификатор техники.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создаёт идентификатор транспортного средства.
	/// </summary>
	/// <param name="value">Значение идентификатора транспортного средства.</param>
	/// <returns>Результат создания идентификатора транспортного средства.</returns>
	public static Result<VehicleId> Create(Guid value)
	{
		return value == Guid.Empty
			? (Result<VehicleId>)Error.Validation("Идентификатор техники не может быть пустым.")
			: (Result<VehicleId>)new VehicleId(value);
	}
}
