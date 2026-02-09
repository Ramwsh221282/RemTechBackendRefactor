namespace ParsersControl.Core.Contracts;

/// <summary>
/// Запрос на получение коллекции подписанных парсеров.
/// </summary>
/// <param name="Domains">Список доменов для фильтрации подписанных парсеров.</param>
/// <param name="Types">Список типов парсеров для фильтрации.</param>
/// <param name="Identifiers">Список идентификаторов парсеров для фильтрации.</param>
/// <param name="WithLock">Флаг, указывающий, нужно ли блокировать записи при получении.</param>
public sealed record SubscribedParsersCollectionQuery(
	IEnumerable<string>? Domains = null,
	IEnumerable<string>? Types = null,
	IEnumerable<Guid>? Identifiers = null,
	bool WithLock = false
);
