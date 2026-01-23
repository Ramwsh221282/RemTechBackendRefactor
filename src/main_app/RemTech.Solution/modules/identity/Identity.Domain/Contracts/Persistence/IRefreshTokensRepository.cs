using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IRefreshTokensRepository
{
	Task<bool> Exists(Guid accountId, CancellationToken ct = default);
	Task Add(RefreshToken token, CancellationToken ct = default);
	Task Update(RefreshToken token, CancellationToken ct = default);
	Task<Result<RefreshToken>> Find(Guid accountId, CancellationToken ct = default);
	Task<Result<RefreshToken>> Find(string refreshToken, bool withLock = false, CancellationToken ct = default);
	Task Delete(RefreshToken token, CancellationToken ct = default);
	Task Delete(Guid AccountId, CancellationToken ct = default);
}
