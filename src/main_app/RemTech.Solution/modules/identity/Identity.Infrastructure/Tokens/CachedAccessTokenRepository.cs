using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Tokens;

public sealed class CachedAccessTokenRepository(HybridCache cache, IAccessTokensRepository inner)
	: IAccessTokensRepository
{
	private HybridCache Cache { get; } = cache;
	private IAccessTokensRepository Inner { get; } = inner;

	public async Task Add(AccessToken token, CancellationToken ct = default)
	{
		await Inner.Add(token, ct);
		string key = token.TokenId.ToString();
		await Cache.SetAsync(key, token, cancellationToken: ct);
	}

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

		return token is null ? Error.NotFound("Token not found.") : token;
	}

	public Task<Result<AccessToken>> Find(string accessToken, bool withLock = false, CancellationToken ct = default) =>
		Inner.Find(accessToken, withLock, ct);

	public Task UpdateTokenExpired(string rawToken, CancellationToken ct = default) =>
		Inner.UpdateTokenExpired(rawToken, ct);

	public Task<IEnumerable<AccessToken>> FindExpired(
		int maxCount = 50,
		bool withLock = false,
		CancellationToken ct = default
	) => Inner.FindExpired(maxCount, withLock, ct);

	public async Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default)
	{
		await Inner.Remove(tokens, ct);
		foreach (AccessToken token in tokens)
		{
			string key = token.TokenId.ToString();
			await Cache.RemoveAsync(key, cancellationToken: ct);
		}
	}

	public async Task Remove(AccessToken token, CancellationToken ct = default)
	{
		await Inner.Remove(token, ct);
		string key = token.TokenId.ToString();
		await Cache.RemoveAsync(key, cancellationToken: ct);
	}

	private Task<Result<AccessToken>> GetFromInner(Guid tokenId, bool withLock, CancellationToken ct) =>
		Inner.Find(tokenId, withLock, ct);
}
