using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

/// <summary>
/// Идентификатор подписки парсера.
/// </summary>
public readonly record struct SubscribedParserId
{
	/// <summary>
	/// Создаёт новый идентификатор подписки парсера.
	/// </summary>
	public SubscribedParserId()
	{
		Value = Guid.NewGuid();
	}

	private SubscribedParserId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение идентификатора.
	/// </summary>
	public Guid Value { get; private init; }

	/// <summary>
	/// Создаёт идентификатор из заданного значения.
	/// </summary>
	/// <param name="value">Значение для создания идентификатора.</param>
	/// <returns>Результат создания идентификатора.</returns>
	public static Result<SubscribedParserId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор подписки парсера не может быть пустым.")
			: new SubscribedParserId(value);
	}

	/// <summary>
	/// Создаёт новый идентификатор подписки парсера.
	/// </summary>
	/// <returns>Новый идентификатор подписки парсера.</returns>
	public static SubscribedParserId New()
	{
		Guid value = Guid.NewGuid();
		return Create(value).Value;
	}
}
