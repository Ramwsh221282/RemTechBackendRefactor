using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Contracts;

/// <summary>
/// Репозиторий коллекций подписанных парсеров.
/// </summary>
public interface ISubscribedParsersCollectionRepository
{
	/// <summary>
	/// Получает коллекцию подписанных парсеров по запросу.
	/// </summary>
	/// <param name="query">Запрос коллекции.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Коллекция подписанных парсеров.</returns>
	Task<SubscribedParsersCollection> Read(SubscribedParsersCollectionQuery query, CancellationToken ct = default);

	/// <summary>
	/// Сохраняет изменения в коллекции подписанных парсеров.
	/// </summary>
	/// <param name="collection">Коллекция для сохранения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат сохранения.</returns>
	Task<Result<Unit>> SaveChanges(SubscribedParsersCollection collection, CancellationToken ct = default);
}
