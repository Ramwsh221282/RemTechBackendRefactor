using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

/// <summary>
/// Статистика парсинга.
/// </summary>
public sealed record ParsingStatistics
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ParsingStatistics"/>.
	/// </summary>
	/// <param name="workTime">Время работы парсинга.</param>
	/// <param name="parsedCount">Количество обработанных элементов.</param>
	public ParsingStatistics(ParsingWorkTime workTime, ParsedCount parsedCount)
	{
		WorkTime = workTime;
		ParsedCount = parsedCount;
	}

	/// <summary>
	/// Время работы парсинга.
	/// </summary>
	public ParsingWorkTime WorkTime { get; private init; }

	/// <summary>
	/// Количество обработанных элементов.
	/// </summary>
	public ParsedCount ParsedCount { get; private init; }

	/// <summary>
	/// Увеличивает количество обработанных элементов на заданное значение.
	/// </summary>
	/// <param name="amount">Значение, на которое нужно увеличить количество обработанных элементов.</param>
	/// <returns>Обновленная статистика парсинга.</returns>
	public Result<ParsingStatistics> IncreaseParsedCount(int amount)
	{
		Result<ParsedCount> updated = ParsedCount.Add(amount);
		return updated.IsFailure ? updated.Error : this with { ParsedCount = updated.Value };
	}

	/// <summary>
	/// Добавляет время работы парсинга.
	/// </summary>
	/// <param name="totalElapsedSeconds">Общее количество прошедших секунд, которое нужно добавить к времени работы парсинга.</param>
	/// <returns>Обновленная статистика парсинга.</returns>
	public Result<ParsingStatistics> AddWorkTime(long totalElapsedSeconds)
	{
		Result<ParsingWorkTime> updated = ParsingWorkTime.FromTotalElapsedSeconds(totalElapsedSeconds);
		return updated.IsFailure ? updated.Error : this with { WorkTime = updated.Value };
	}

	/// <summary>
	/// Сбросить время работы парсера.
	/// </summary>
	/// <returns>Обновленная статистика парсинга с сброшенным временем работы.</returns>
	public ParsingStatistics ResetWorkTime() => this with { WorkTime = ParsingWorkTime.New() };

	/// <summary>
	/// Сбросить количество обработанных элементов.
	/// </summary>
	/// <returns>Обновленная статистика парсинга с сброшенным количеством обработанных элементов.</returns>
	public ParsingStatistics ResetParsedCount() => this with { ParsedCount = ParsedCount.New() };

	/// <summary>
	/// Создает новую статистику парсинга с нулевыми значениями.
	/// </summary>
	/// <returns>Новая статистика парсинга с нулевыми значениями.</returns>
	public static ParsingStatistics New()
	{
		ParsingWorkTime workTime = ParsingWorkTime.New();
		ParsedCount parsedCount = ParsedCount.New();
		return new ParsingStatistics(workTime, parsedCount);
	}
}
