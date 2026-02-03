using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Tokens;

public sealed class CachedRefreshTokensRepository : IRefreshTokensRepository
{
	private const string KEY_PREFIX_WITH_ACCOUNT_ID = "rt_by_account";
	private const string KEY_FOR_RAW_TOKEN = "rt";
	private readonly HybridCacheEntryOptions _options = new()
	{
		Expiration = TimeSpan.FromMinutes(10),
		LocalCacheExpiration = TimeSpan.FromMinutes(10),
	};
	private readonly HybridCache _cache;
	private readonly IRefreshTokensRepository _inner;

	public CachedRefreshTokensRepository(HybridCache cache, IRefreshTokensRepository inner)
	{
		_cache = cache;
		_inner = inner;
	}

	public Task Add(RefreshToken token, CancellationToken ct = default)
	{
		Console.WriteLine("Adding token to cache repository");
		return _inner.Add(token, ct);
	}

	public async Task Delete(RefreshToken token, CancellationToken ct = default)
	{
		Console.WriteLine("Deleting token from cache repository. Token instance provided.");
		await _inner.Delete(token, ct);
		await InvalidateTokens(token, ct);
	}

	public async Task Delete(Guid accountId, CancellationToken ct = default)
	{
		Console.WriteLine("Deleting token from cache repository. AccountId provided.");
		await _inner.Delete(accountId, ct);
		CachedRefreshToken? cachedToken = await GetFromCacheByAccountId(accountId, ct);
		if (cachedToken is not null)
		{
			await InvalidateTokens(cachedToken.AccountId, cachedToken.TokenValue, ct);
		}
	}

	public async Task<bool> Exists(Guid accountId, CancellationToken ct = default)
	{
		Console.WriteLine("Checking token existence in cache repository.");
		CachedRefreshToken? cachedToken = await GetFromCacheByAccountId(accountId, ct);
		if (cachedToken is not null)
		{
			return true;
		}

		return await _inner.Exists(accountId, ct);
	}

	public async Task<Result<RefreshToken>> Find(Guid accountId, CancellationToken ct = default)
	{
		Console.WriteLine("Finding token by accountId in cache repository.");
		CachedRefreshToken? cachedToken = await GetFromCacheByAccountId(accountId, ct);
		if (cachedToken is not null)
		{
			return MapFromCached(cachedToken);
		}

		return await _inner.Find(accountId, ct);
	}

	public async Task<Result<RefreshToken>> Find(
		string refreshToken,
		bool withLock = false,
		CancellationToken ct = default
	)
	{
		Console.WriteLine("Finding token by token value in cache repository.");
		if (withLock)
		{
			Console.WriteLine("With lock is requested, bypassing cache.");
			return await _inner.Find(refreshToken, withLock, ct);
		}

		CachedRefreshToken? cachedToken = await GetFromCacheByTokenValue(refreshToken, ct);
		return cachedToken is null ? await _inner.Find(refreshToken, withLock, ct) : MapFromCached(cachedToken);
	}

	public async Task Update(RefreshToken token, CancellationToken ct = default)
	{
		Console.WriteLine("Updating token in cache repository.");
		await _inner.Update(token, ct);
		await InvalidateTokens(token, ct);
	}

	private async Task InvalidateTokens(Guid accountId, string tokenValue, CancellationToken ct)
	{
		string keyWithAccountId = CreateCacheKeyWithAccountId(accountId);
		string keyForRawToken = CreateCacheKeyForRawToken(tokenValue);
		await _cache.RemoveAsync(keyWithAccountId, ct);
		await _cache.RemoveAsync(keyForRawToken, ct);
	}

	private async Task InvalidateTokens(RefreshToken token, CancellationToken ct)
	{
		string keyWithAccountId = CreateCacheKeyWithAccountId(token);
		string keyForRawToken = CreateCacheKeyForRawToken(token);
		await _cache.RemoveAsync(keyWithAccountId, ct);
		await _cache.RemoveAsync(keyForRawToken, ct);
	}

	private sealed record CachedRefreshToken(Guid AccountId, string TokenValue, long ExpiresAt, long CreatedAt);

	private async Task<CachedRefreshToken?> GetFromCacheByTokenValue(string tokenValue, CancellationToken ct)
	{
		string key = CreateCacheKeyForRawToken(tokenValue);
		return await _cache.GetOrCreateAsync<CachedRefreshToken?>(
			key,
			async (ct) => null,
			_options,
			cancellationToken: ct
		);
	}

	private async Task<CachedRefreshToken?> GetFromCacheByAccountId(Guid accountId, CancellationToken ct)
	{
		string key = CreateCacheKeyWithAccountId(accountId);
		return await _cache.GetOrCreateAsync<CachedRefreshToken?>(
			key,
			async (ct) => null,
			_options,
			cancellationToken: ct
		);
	}

	private static string CreateCacheKeyWithAccountId(RefreshToken token)
	{
		return CreateCacheKeyWithAccountId(token.AccountId);
	}

	private static string CreateCacheKeyWithAccountId(Guid accountId)
	{
		return $"{KEY_PREFIX_WITH_ACCOUNT_ID}_{accountId}";
	}

	private static string CreateCacheKeyForRawToken(RefreshToken token)
	{
		return CreateCacheKeyForRawToken(token.TokenValue);
	}

	private static string CreateCacheKeyForRawToken(string tokenValue)
	{
		return $"{KEY_FOR_RAW_TOKEN}_{tokenValue}";
	}

	private static RefreshToken MapFromCached(CachedRefreshToken token)
	{
		return new RefreshToken(token.AccountId, token.TokenValue, token.ExpiresAt, token.CreatedAt);
	}
}
