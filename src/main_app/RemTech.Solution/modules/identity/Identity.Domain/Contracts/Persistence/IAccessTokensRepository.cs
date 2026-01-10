using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccessTokensRepository
{
    Task Add(AccessToken token, CancellationToken ct = default);
    Task<Result<AccessToken>> Get(Guid tokenId, bool withLock = false, CancellationToken ct = default);
    Task<Result<AccessToken>> Get(string accessToken, bool withLock = false, CancellationToken ct = default);
    Task<Guid?> Remove(string accessToken, CancellationToken ct = default);
    Task UpdateTokenExpired(string rawToken, CancellationToken ct = default);
    Task<IEnumerable<AccessToken>> GetExpired(int maxCount = 50, bool withLock = false, CancellationToken ct = default);
    Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default);
}