using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IRefreshTokensRepository
{
	public Task<bool> Exists(Guid accountId, CancellationToken ct = default);
	public Task Add(RefreshToken token, CancellationToken ct = default);
	public Task Update(RefreshToken token, CancellationToken ct = default);
	public Task<Result<RefreshToken>> Find(Guid accountId, CancellationToken ct = default);
	public Task<Result<RefreshToken>> Find(string refreshToken, bool withLock = false, CancellationToken ct = default);
	public Task Delete(RefreshToken token, CancellationToken ct = default);
	public Task Delete(Guid AccountId, CancellationToken ct = default);
}
