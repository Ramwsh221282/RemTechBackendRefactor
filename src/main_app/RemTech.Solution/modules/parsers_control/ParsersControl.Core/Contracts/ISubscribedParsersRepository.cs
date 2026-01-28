using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Contracts;

/// <summary>
/// Репозиторий подписанных парсеров.
/// </summary>
public interface ISubscribedParsersRepository
{
	/// <summary>
	/// Проверяет существование подписанного парсера по идентификатору.
	/// </summary>
	/// <param name="identity">Идентификатор парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>True, если парсер существует.</returns>
	Task<bool> Exists(SubscribedParserIdentity identity, CancellationToken ct = default);

	/// <summary>
	/// Добавляет подписанный парсер.
	/// </summary>
	/// <param name="parser">Экземпляр парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача добавления.</returns>
	Task Add(SubscribedParser parser, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет коллекцию подписанных парсеров.
	/// </summary>
	/// <param name="parsers">Коллекция парсеров.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача сохранения.</returns>
	Task Save(IEnumerable<SubscribedParser> parsers, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет подписанный парсер.
	/// </summary>
	/// <param name="parser">Экземпляр парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача сохранения.</returns>
	Task Save(SubscribedParser parser, CancellationToken ct = default);

	/// <summary>
	/// Получает подписанный парсер по запросу.
	/// </summary>
	/// <param name="query">Запрос парсера.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат получения парсера.</returns>
	Task<Result<SubscribedParser>> Get(SubscribedParserQuery query, CancellationToken ct = default);
}
