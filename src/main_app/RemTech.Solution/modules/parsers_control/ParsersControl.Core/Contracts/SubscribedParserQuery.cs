namespace ParsersControl.Core.Contracts;

/// <summary>
/// Запрос для получения подписанного парсера.
/// </summary>
/// <param name="Domain">Домен парсера.</param>
/// <param name="Type">Тип парсера.</param>
/// <param name="Id">Идентификатор парсера.</param>
/// <param name="WithLock">Флаг блокировки.</param>
public sealed record SubscribedParserQuery(
	string? Domain = null,
	string? Type = null,
	Guid? Id = null,
	bool WithLock = false
)
{
	/// <summary>
	/// Создаёт запрос с заданным идентификатором.
	/// </summary>
	/// <param name="id">Идентификатор парсера.</param>
	/// <returns>Новый запрос с заданным идентификатором.</returns>
	public SubscribedParserQuery WithId(Guid id) => Id.HasValue ? this : this with { Id = id };

	/// <summary>
	/// Создаёт запрос с заданным доменом.
	/// </summary>
	/// <param name="domain">Домен парсера.</param>
	/// <returns>Новый запрос с заданным доменом.</returns>
	public SubscribedParserQuery OfDomain(string domain) =>
		!string.IsNullOrWhiteSpace(Domain) ? this : this with { Domain = domain };

	/// <summary>
	/// Создаёт запрос с заданным типом.
	/// </summary>
	/// <param name="type">Тип парсера.</param>
	/// <returns>Новый запрос с заданным типом.</returns>
	public SubscribedParserQuery OfType(string type) =>
		!string.IsNullOrWhiteSpace(Type) ? this : this with { Type = type };

	/// <summary>
	/// Создаёт запрос с требованием блокировки.
	/// </summary>
	/// <returns>Новый запрос с требованием блокировки.</returns>
	public SubscribedParserQuery RequireLock() => this with { WithLock = true };
}
