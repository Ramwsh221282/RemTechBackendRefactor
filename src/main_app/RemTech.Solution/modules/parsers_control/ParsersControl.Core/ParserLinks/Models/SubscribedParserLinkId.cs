using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinks.Models;

/// <summary>
/// Идентификатор ссылки на подписанный парсер.
/// </summary>
public readonly record struct SubscribedParserLinkId
{
	/// <summary>
	/// Создаёт новый идентификатор ссылки на подписанный парсер.
	/// </summary>
	public SubscribedParserLinkId()
	{
		Value = Guid.NewGuid();
	}

	/// <summary>
	/// Создаёт идентификатор ссылки на подписанный парсер из заданного значения.
	/// </summary>
	/// <param name="id">Значение идентификатора.</param>
	private SubscribedParserLinkId(Guid id)
	{
		Value = id;
	}

	/// <summary>
	/// Значение идентификатора ссылки на подписанный парсер.
	/// </summary>
	public Guid Value { get; private init; }

	/// <summary>
	/// Создаёт идентификатор ссылки на подписанный парсер из заданного значения.
	/// </summary>
	/// <param name="id">Значение идентификатора.</param>
	/// <returns>Результат создания идентификатора ссылки на подписанный парсер.</returns>
	public static Result<SubscribedParserLinkId> From(Guid id)
	{
		return id == Guid.Empty
			? Error.Validation("Идентификатор ссылки на парсер не может быть пустым.")
			: new SubscribedParserLinkId(id);
	}

	/// <summary>
	/// Создаёт новый идентификатор ссылки на подписанный парсер.
	/// </summary>
	/// <returns>Новый идентификатор ссылки на подписанный парсер.</returns>
	public static SubscribedParserLinkId New()
	{
		return new();
	}
}
