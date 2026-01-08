using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccessTokensRepository
{
    Task Add(AccessToken token, CancellationToken ct = default);
    Task<Result<AccessToken>> Get(Guid tokenId, CancellationToken ct = default);
    Task<Result<AccessToken>> Get(string accessToken, CancellationToken ct = default);
    Task<Guid?> Remove(string accessToken, CancellationToken ct = default);
}