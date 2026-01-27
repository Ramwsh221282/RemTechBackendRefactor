using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

/// <summary>
/// Время работы парсера.
/// </summary>
public sealed record ParsingWorkTime
{
	private const long INITIAL_VALUE = 0;

	private ParsingWorkTime(long totalElapsedSeconds)
	{
		TotalElapsedSeconds = totalElapsedSeconds;
	}

	/// <summary>
	/// Общее количество секунд работы парсера.
	/// </summary>
	public long TotalElapsedSeconds { get; private init; }

	/// <summary>
	/// Часы работы парсера.
	/// </summary>
	public int Hours => (int)(TotalElapsedSeconds / 3600);

	/// <summary>
	/// Минуты работы парсера.
	/// </summary>
	public int Minutes => (int)((TotalElapsedSeconds % 3600) / 60);

	/// <summary>
	/// Секунды работы парсера.
	/// </summary>
	public int Seconds => (int)(TotalElapsedSeconds % 60);

	/// <summary>
	/// Создаёт новый экземпляр времени работы парсера с нулевым значением.
	/// </summary>
	/// <returns>Экземпляр ParsingWorkTime.</returns>
	public static ParsingWorkTime New() => new(INITIAL_VALUE);

	/// <summary>
	/// Создаёт экземпляр времени работы парсера по общему количеству секунд.
	/// </summary>
	/// <param name="totalElapsedSeconds">Общее количество секунд.</param>
	/// <returns>Результат создания времени работы парсера.</returns>
	public static Result<ParsingWorkTime> FromTotalElapsedSeconds(long totalElapsedSeconds)
	{
		return totalElapsedSeconds < INITIAL_VALUE
			? Error.Validation("Общее время работы парсера не может быть отрицательным.")
			: new ParsingWorkTime(totalElapsedSeconds);
	}
}
