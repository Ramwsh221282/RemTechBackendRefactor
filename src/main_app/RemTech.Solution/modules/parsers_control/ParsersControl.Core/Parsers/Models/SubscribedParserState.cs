using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

/// <summary>
/// Состояние работы подписанного парсера.
/// </summary>
public sealed record SubscribedParserState
{
	private const string SLEEPING = "Ожидает";
	private const string WORKING = "В работе";
	private const string DISABLED = "Отключен";
	private static readonly string[] _states = [SLEEPING, WORKING, DISABLED];

	/// <summary>
	/// Создаёт состояние работы парсера из строки.
	/// </summary>
	/// <param name="value">Значение состояния работы парсера.</param>
	private SubscribedParserState(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение состояния работы парсера.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Предопределённые состояния работы парсера.
	/// </summary>
	public static SubscribedParserState Sleeping => new(SLEEPING);

	/// <summary>
	/// Предопределённые состояния работы парсера.
	/// </summary>
	public static SubscribedParserState Working => new(WORKING);

	/// <summary>
	/// Предопределённые состояния работы парсера.
	/// </summary>
	public static SubscribedParserState Disabled => new(DISABLED);

	/// <summary>
	/// Создаёт состояние работы парсера из строки.
	/// </summary>
	/// <param name="input">Строковое представление состояния работы парсера.</param>
	/// <returns>Результат с созданным состоянием работы парсера или ошибкой.</returns>
	public static Result<SubscribedParserState> FromString(string input)
	{
		string? state = _states.FirstOrDefault(s => s == input);
		return state == null ? Error.InvalidFormat("Состояние работы парсера") : new SubscribedParserState(state);
	}

	/// <summary>
	/// Проверяет, совпадает ли текущее состояние с другим состоянием.
	/// </summary>
	/// <param name="other">Другое состояние работы парсера для сравнения.</param>
	/// <returns>True, если состояния совпадают; в противном случае false.</returns>
	public bool HasSameState(SubscribedParserState other)
	{
		return Value == other.Value;
	}

	/// <summary>
	/// Проверяет, находится ли парсер в рабочем состоянии.
	/// </summary>
	/// <returns>True, если парсер находится в рабочем состоянии; в противном случае false.</returns>
	public bool IsWorking()
	{
		return Value == WORKING;
	}

	/// <summary>
	/// Проверяет, отключен ли парсер.
	/// </summary>
	/// <returns>True, если парсер отключен; в противном случае false.</returns>
	public bool IsDisabled()
	{
		return Value == DISABLED;
	}

	/// <summary>
	/// Проверяет, находится ли парсер в состоянии ожидания.
	/// </summary>
	/// <returns>True, если парсер находится в состоянии ожидания; в противном случае false.</returns>
	public bool IsSleeping()
	{
		return Value == SLEEPING;
	}
}
