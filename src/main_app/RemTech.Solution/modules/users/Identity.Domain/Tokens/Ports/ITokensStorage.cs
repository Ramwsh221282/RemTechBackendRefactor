using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users;

namespace Identity.Domain.Tokens.Ports;

public interface ITokensStorage
{
    Task<UserToken> CreateToken(User user, CancellationToken ct = default);
    Task<UserToken?> Get(Token token, RoleName role, CancellationToken ct = default);
    Task<UserToken?> Get(Token token, CancellationToken ct = default);
}
