using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Информация о цене транспортного средства.
/// </summary>
public sealed record VehiclePriceInformation
{
	private VehiclePriceInformation(long value, bool isNds)
	{
		Value = value;
		IsNds = isNds;
	}

	/// <summary>
	/// Цена транспортного средства.
	/// </summary>
	public long Value { get; }

	/// <summary>
	/// Включен НДС или нет.
	/// </summary>
	public bool IsNds { get; }

	/// <summary>
	/// Создаёт информацию о цене транспортного средства.
	/// </summary>
	/// <param name="value">Цена транспортного средства.</param>
	/// <param name="isNds">Включен НДС или нет.</param>
	/// <returns>Результат создания информации о цене транспортного средства.</returns>
	public static Result<VehiclePriceInformation> Create(long value, bool isNds)
	{
		return value <= 0
			? (Result<VehiclePriceInformation>)Error.Validation("Цена техники не может быть меньше или равной нулю.")
			: (Result<VehiclePriceInformation>)new VehiclePriceInformation(value, isNds);
	}
}
