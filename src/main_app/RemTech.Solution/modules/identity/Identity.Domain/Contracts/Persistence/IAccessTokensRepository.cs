using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccessTokensRepository
{
	public Task Add(AccessToken token, CancellationToken ct = default);
	public Task<Result<AccessToken>> Find(Guid tokenId, bool withLock = false, CancellationToken ct = default);
	public Task<Result<AccessToken>> Find(string accessToken, bool withLock = false, CancellationToken ct = default);
	public Task UpdateTokenExpired(string rawToken, CancellationToken ct = default);
	public Task<IEnumerable<AccessToken>> FindExpired(
		int maxCount = 50,
		bool withLock = false,
		CancellationToken ct = default
	);
	public Task Remove(IEnumerable<AccessToken> tokens, CancellationToken ct = default);
	public Task Remove(AccessToken token, CancellationToken ct = default);
}
