using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Tokens;

/// <summary>
/// Кэшированный репозиторий для управления токенами доступа.
/// </summary>
/// <param name="cache">Кэш для хранения токенов доступа.</param>
/// <param name="inner">Внутренний репозиторий для управления токенами доступа.</param>
public sealed class CachedAccessTokenRepository(HybridCache cache, IAccessTokensRepository inner)
	: IAccessTokensRepository
{
	private HybridCache Cache { get; } = cache;
	private IAccessTokensRepository Inner { get; } = inner;

	/// <summary>
	/// Добавляет токен доступа в репозиторий и кэш.
	/// </summary>
	/// <param name="token">Токен доступа для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления токена.</returns>
	public async Task Add(AccessToken token, CancellationToken ct = default)
	{
		await Inner.Add(token, ct);
		string key = token.TokenId.ToString();
		await Cache.SetAsync(key, token, cancellationToken: ct);
	}

	/// <summary>
	/// Находит токен доступа по его идентификатору, используя кэш.
	/// </summary>
	/// <param name="tokenId">Идентификатор токена доступа.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать запись для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции, содержащий найденный токен доступа или ошибку.</returns>
	public async Task<Result<AccessToken>> Find(Guid tokenId, bool withLock = false, CancellationToken ct = default)
	{
		string key = tokenId.ToString();

		AccessToken? token = await Cache.GetOrCreateAsync(
			key,
			async cancellationToken =>
			{
				Result<AccessToken> result = await GetFromInner(tokenId, withLock, cancellationToken);
				return result.IsFailure ? null : result.Value;
			},
			cancellationToken: ct
		);

		return token is null ? Error.NotFound("Токен не найден.") : token;
	}

	/// <summary>
	/// Находит токен доступа по его значению.
	/// </summary>
	/// <param name="accessToken">Значение токена доступа.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать запись для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат операции, содержащий найденный токен доступа или ошибку.</returns>
	public Task<Result<AccessToken>> Find(string accessToken, bool withLock = false, CancellationToken ct = default) =>
		Inner.Find(accessToken, withLock, ct);

	/// <summary>
	/// Обновляет статус истечения срока действия токена доступа.
	/// </summary>
	/// <param name="rawToken">Значение токена доступа.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию.</returns>
	public Task UpdateTokenExpired(string rawToken, CancellationToken ct = default) =>
		Inner.UpdateTokenExpired(rawToken, ct);

	/// <summary>
	/// Находит истекшие токены доступа.
	/// </summary>
	/// <param name="maxCount">Максимальное количество токенов для поиска.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать записи для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция найденных истекших токенов доступа.</returns>
	public Task<IEnumerable<AccessToken>> FindExpired(
		int maxCount = 50,
		bool withLock = false,
		CancellationToken ct = default
	) => Inner.FindExpired(maxCount, withLock, ct);

	/// <summary>
	/// Удаляет заданные токены доступа из репозитория и кэша.
	/// </summary>
	/// <param name="tokens">Коллекция токенов доступа для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию удаления токенов.</returns>
	public async Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default)
	{
		await Inner.Remove(tokens, ct);
		foreach (AccessToken token in tokens)
		{
			string key = token.TokenId.ToString();
			await Cache.RemoveAsync(key, cancellationToken: ct);
		}
	}

	/// <summary>
	/// Удаляет заданный токен доступа из репозитория и кэша.
	/// </summary>
	/// <param name="token">Токен доступа для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию удаления токена.</returns>
	public async Task Remove(AccessToken token, CancellationToken ct = default)
	{
		await Inner.Remove(token, ct);
		string key = token.TokenId.ToString();
		await Cache.RemoveAsync(key, cancellationToken: ct);
	}

	private Task<Result<AccessToken>> GetFromInner(Guid tokenId, bool withLock, CancellationToken ct) =>
		Inner.Find(tokenId, withLock, ct);
}
