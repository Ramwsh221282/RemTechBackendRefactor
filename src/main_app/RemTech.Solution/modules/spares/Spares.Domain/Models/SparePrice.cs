using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Цена запчасти.
/// </summary>
public sealed record SparePrice
{
	/// <summary>
	/// Создаёт экземпляр цены запчасти.
	/// </summary>
	/// <param name="value">Значение цены.</param>
	/// <param name="isNds">Признак наличия НДС.</param>
	private SparePrice(long value, bool isNds)
	{
		(Value, IsNds) = (value, isNds);
	}

	/// <summary>
	/// Значение цены.
	/// </summary>
	public long Value { get; }

	/// <summary>
	/// Признак наличия НДС.
	/// </summary>
	public bool IsNds { get; }

	/// <summary>
	/// Создаёт цену запчасти.
	/// </summary>
	/// <param name="value">Значение цены.</param>
	/// <param name="isNds">Признак наличия НДС.</param>
	/// <returns>Результат создания цены.</returns>
	public static Result<SparePrice> Create(long value, bool isNds) =>
		value <= 0
			? Error.Validation("Цена запчасти должна быть больше 0.")
			: Result.Success(new SparePrice(value, isNds));
}
