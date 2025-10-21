using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users;
using Identity.Domain.Users.Aggregate;

namespace Identity.Domain.Tokens.Ports;

public interface ITokensStorage
{
    Task<UserToken> CreateToken(IdentityUser identityUser, CancellationToken ct = default);
    Task<UserToken?> Get(Token token, RoleName role, CancellationToken ct = default);
    Task<UserToken?> Get(Token token, CancellationToken ct = default);
    Task Remove(Token token, CancellationToken ct);
}
