using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

/// <summary>
/// Количество обработанных данных парсером.
/// </summary>
public readonly record struct ParsedCount
{
	/// <summary>
	/// Создаёт новый счётчик с нулевым значением.
	/// </summary>
	public ParsedCount()
	{
		Value = 0;
	}

	/// <summary>
	/// Создаёт счётчик с заданным значением.
	/// </summary>
	/// <param name="value">Значение количества обработанных данных парсером.</param>
	private ParsedCount(int value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение количества обработанных данных парсером.
	/// </summary>
	public int Value { get; private init; }

	/// <summary>
	/// Создаёт счётчик из заданного значения.
	/// </summary>
	/// <param name="value">Значение количества обработанных данных парсером.</param>
	/// <returns>Результат создания счётчика.</returns>
	public static Result<ParsedCount> Create(int value) =>
		value < 0
			? Error.Validation("Количество обработанных данных парсером не может быть отрицательным.")
			: new ParsedCount(value);

	/// <summary>
	/// Создаёт новый счётчик с нулевым значением.
	/// </summary>
	/// <returns>Новый счётчик с нулевым значением.</returns>
	public static ParsedCount New() => Create(0).Value;

	/// <summary>
	/// Добавляет заданное количество к текущему счётчику.
	/// </summary>
	/// <param name="amount">Количество, которое нужно добавить к текущему счётчику.</param>
	/// <returns>Новый счётчик с обновлённым значением.</returns>
	public Result<ParsedCount> Add(int amount) =>
		amount < 0
			? Error.Validation("Количество добавляемых данных парсером не может быть отрицательным.")
			: new ParsedCount(Value + amount);
}
