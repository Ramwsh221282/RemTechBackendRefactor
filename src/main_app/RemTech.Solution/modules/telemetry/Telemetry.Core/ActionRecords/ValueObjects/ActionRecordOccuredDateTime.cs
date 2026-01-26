using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Дата и время возникновения действия.
/// </summary>
public readonly record struct ActionRecordOccuredDateTime
{
	/// <summary>
	/// Создает дату и время возникновения действия.
	/// </summary>
	public ActionRecordOccuredDateTime()
	{
		Value = DateTime.UtcNow;
	}

	private ActionRecordOccuredDateTime(DateTime date)
	{
		Value = date;
	}

	/// <summary>
	/// Значение даты и времени возникновения действия.
	/// </summary>
	public DateTime Value { get; }

	/// <summary>
	///  Создает дату и время возникновения действия.
	/// </summary>
	/// <returns>Дата и время возникновения действия.</returns>
	public static ActionRecordOccuredDateTime Now() => new();

	/// <summary>
	/// Создает дату и время возникновения действия.
	/// </summary>
	/// <param name="date">Дата и время возникновения действия.</param>
	/// <returns>Результат создания даты и времени возникновения действия.</returns>
	public static Result<ActionRecordOccuredDateTime> Create(DateTime date)
	{
		if (date == DateTime.MinValue || date == DateTime.MaxValue)
		{
			return Error.Validation("Некорректная дата и время возникновения действия.");
		}

		return new ActionRecordOccuredDateTime(date);
	}
}
