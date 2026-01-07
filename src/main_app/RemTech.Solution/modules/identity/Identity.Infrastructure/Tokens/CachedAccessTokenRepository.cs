using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Tokens;

public sealed class CachedAccessTokenRepository(HybridCache cache, IAccessTokensRepository inner) : IAccessTokensRepository
{
    private HybridCache Cache { get; } = cache;
    private IAccessTokensRepository Inner { get; } = inner;
    
    public async Task Add(AccessToken token, CancellationToken ct = default)
    {
        await Inner.Add(token, ct);
        string key = token.TokenId.ToString();
        await Cache.SetAsync(key, token, cancellationToken: ct);
    }

    public async Task<Result<AccessToken>> Get(Guid tokenId, CancellationToken ct = default)
    {
        string key = tokenId.ToString();

        AccessToken? token = await Cache.GetOrCreateAsync(
            key,
            async cancellationToken =>
            {
                Result<AccessToken> result = await GetFromInner(tokenId, cancellationToken);
                if (result.IsFailure) return null;
                return result.Value;
            },
            cancellationToken: ct);
        
        return token is null ? Error.NotFound("Token not found.") : token;
    }

    public async Task<Result<AccessToken>> Get(string accessToken, CancellationToken ct = default)
    {
        return await Inner.Get(accessToken, ct);
    }

    private async Task<Result<AccessToken>> GetFromInner(Guid tokenId, CancellationToken ct)
    {
        return await Inner.Get(tokenId, ct);
    }
}