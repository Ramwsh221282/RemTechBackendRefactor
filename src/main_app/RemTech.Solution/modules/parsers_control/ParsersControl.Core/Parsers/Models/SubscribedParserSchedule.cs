using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

/// <summary>
/// Расписание подписки парсера.
/// </summary>
public readonly record struct SubscribedParserSchedule
{
	private const int MAX_WAIT_DAYS_AMOUNT = 7;

	/// <summary>
	/// 	Создаёт новое расписание подписки парсера с пустыми значениями.
	/// </summary>
	public SubscribedParserSchedule()
	{
		StartedAt = null;
		FinishedAt = null;
		WaitDays = null;
	}

	/// <summary>
	/// Создаёт расписание подписки парсера с заданными значениями.
	/// </summary>
	/// <param name="startedAt">Время начала подписки парсера.</param>
	/// <param name="finishedAt">Время окончания подписки парсера.</param>
	/// <param name="nextRun">Время следующего запуска парсера.</param>
	/// <param name="waitDays">Количество дней ожидания перед следующим запуском.</param>
	private SubscribedParserSchedule(DateTime? startedAt, DateTime? finishedAt, DateTime? nextRun, int? waitDays)
	{
		StartedAt = startedAt;
		FinishedAt = finishedAt;
		NextRun = nextRun;
		WaitDays = waitDays;
	}

	/// <summary>
	/// Время начала подписки парсера.
	/// </summary>
	public DateTime? StartedAt { get; private init; }

	/// <summary>
	/// Время окончания подписки парсера.
	/// </summary>
	public DateTime? FinishedAt { get; private init; }

	/// <summary>
	/// Время следующего запуска парсера.
	/// </summary>
	public DateTime? NextRun { get; private init; }

	/// <summary>
	/// Количество дней ожидания перед следующим запуском.
	/// </summary>
	public int? WaitDays { get; private init; }

	/// <summary>
	/// Устанавливает время начала подписки парсера.
	/// </summary>
	/// <param name="startedAt">Время начала подписки парсера.</param>
	/// <returns>Результат с обновлённым расписанием подписки парсера.</returns>
	public Result<SubscribedParserSchedule> WithStartedAt(DateTime startedAt) =>
		Create(startedAt, FinishedAt, NextRun, WaitDays);

	/// <summary>
	/// Устанавливает время следующего запуска парсера.
	/// </summary>
	/// <param name="nextRun">Время следующего запуска парсера.</param>
	/// <returns>Результат с обновлённым расписанием подписки парсера.</returns>
	public SubscribedParserSchedule WithNextRun(DateTime nextRun) => Create(StartedAt, FinishedAt, nextRun, WaitDays);

	/// <summary>
	/// Устанавливает время окончания подписки парсера.
	/// </summary>
	/// <param name="finishedAt">Время окончания подписки парсера.</param>
	/// <returns>Результат с обновлённым расписанием подписки парсера.</returns>
	public Result<SubscribedParserSchedule> WithFinishedAt(DateTime finishedAt)
	{
		Result<SubscribedParserSchedule> result = Create(StartedAt, finishedAt, NextRun, WaitDays);
		return result.IsFailure
			? (Result<SubscribedParserSchedule>)result.Error
			: (Result<SubscribedParserSchedule>)result.Value.AdjustNextRun();
	}

	/// <summary>
	/// Устанавливает количество дней ожидания перед следующим запуском.
	/// </summary>
	/// <param name="waitDays">Количество дней ожидания перед следующим запуском.</param>
	/// <returns>Результат с обновлённым расписанием подписки парсера.</returns>
	public Result<SubscribedParserSchedule> WithWaitDays(int waitDays) =>
		Create(StartedAt, FinishedAt, NextRun, waitDays);

	/// <summary>
	/// Создаёт новое расписание подписки парсера с одним днём ожидания.
	/// </summary>
	/// <returns>Новое расписание подписки парсера с одним днём ожидания.</returns>
	public static SubscribedParserSchedule New() => new() { WaitDays = 1 };

	/// <summary>
	/// Создаёт расписание подписки парсера с заданными значениями.
	/// </summary>
	/// <param name="startedAt">Время начала подписки парсера.</param>
	/// <param name="finishedAt">Время окончания подписки парсера.</param>
	/// <param name="nextRun">Время следующего запуска парсера.</param>
	/// <param name="waitDays">Количество дней ожидания перед следующим запуском.</param>
	/// <returns>Результат с созданным расписанием подписки парсера.</returns>
	public static Result<SubscribedParserSchedule> Create(
		DateTime? startedAt,
		DateTime? finishedAt,
		DateTime? nextRun,
		int? waitDays
	)
	{
		if (waitDays != null && waitDays.Value is < 1 or > MAX_WAIT_DAYS_AMOUNT)
			return Error.Validation($"Дни ожидания не могут быть менее 1 или более {MAX_WAIT_DAYS_AMOUNT}");

		return new SubscribedParserSchedule(startedAt, finishedAt, nextRun, waitDays);
	}

	private SubscribedParserSchedule AdjustNextRun()
	{
		if (WaitDays == null || FinishedAt == null)
			return this;
		DateTime newNextRun = FinishedAt.Value.AddDays(WaitDays.Value);
		return Create(StartedAt, FinishedAt, newNextRun, WaitDays);
	}
}
