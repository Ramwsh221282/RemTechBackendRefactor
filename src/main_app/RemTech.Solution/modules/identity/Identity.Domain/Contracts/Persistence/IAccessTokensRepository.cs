using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccessTokensRepository
{
	Task Add(AccessToken token, CancellationToken ct = default);
	Task<Result<AccessToken>> Find(Guid tokenId, bool withLock = false, CancellationToken ct = default);
	Task<Result<AccessToken>> Find(string accessToken, bool withLock = false, CancellationToken ct = default);
	Task UpdateTokenExpired(string rawToken, CancellationToken ct = default);
	Task<IEnumerable<AccessToken>> FindExpired(
		int maxCount = 50,
		bool withLock = false,
		CancellationToken ct = default
	);
	Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default);
	Task Remove(AccessToken token, CancellationToken ct = default);
}
