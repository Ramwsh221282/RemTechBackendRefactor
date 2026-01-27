using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Интерфейс репозитория для управления токенами доступа.
/// </summary>
public interface IAccessTokensRepository
{
	/// <summary>
	/// Добавляет новый токен доступа в репозиторий.
	/// </summary>
	/// <param name="token">Токен доступа для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Добавить новый токен доступа.</returns>
	Task Add(AccessToken token, CancellationToken ct = default);

	/// <summary>
	/// Находит токен доступа по его идентификатору.
	/// </summary>
	/// <param name="tokenId">Идентификатор токена доступа.</param>
	/// <param name="withLock">Флаг, указывающий на необходимость блокировки записи при выборке.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции с найденным токеном доступа.</returns>
	Task<Result<AccessToken>> Find(Guid tokenId, bool withLock = false, CancellationToken ct = default);

	/// <summary>
	/// Находит токен доступа по его строковому представлению.
	/// </summary>
	/// <param name="accessToken">Строковое представление токена доступа.</param>
	/// <param name="withLock">Флаг, указывающий на необходимость блокировки записи при выборке.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции с найденным токеном доступа.</returns>
	Task<Result<AccessToken>> Find(string accessToken, bool withLock = false, CancellationToken ct = default);

	/// <summary>
	/// Обновляет статус истечения срока действия токена.
	/// </summary>
	/// <param name="rawToken">Строковое представление токена доступа.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task UpdateTokenExpired(string rawToken, CancellationToken ct = default);

	/// <summary>
	/// Находит просроченные токены доступа.
	/// </summary>
	/// <param name="maxCount">Максимальное количество токенов для выборки.</param>
	/// <param name="withLock">Флаг, указывающий на необходимость блокировки записи при выборке.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция просроченных токенов доступа.</returns>
	Task<IEnumerable<AccessToken>> FindExpired(
		int maxCount = 50,
		bool withLock = false,
		CancellationToken ct = default
	);

	/// <summary>
	/// Удаляет коллекцию токенов доступа.
	/// </summary>
	/// <param name="tokens">Коллекция токенов доступа для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default);

	/// <summary>
	/// Удаляет токен доступа.
	/// </summary>
	/// <param name="token">Токен доступа для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Remove(AccessToken token, CancellationToken ct = default);
}
