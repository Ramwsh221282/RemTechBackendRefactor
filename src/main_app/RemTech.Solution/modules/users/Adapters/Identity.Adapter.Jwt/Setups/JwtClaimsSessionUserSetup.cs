using Identity.Adapter.Jwt.Claims;
using Identity.Adapter.Jwt.Users;
using Identity.Jwt.Port;

namespace Identity.Adapter.Jwt.Setups;

/// <summary>
/// Установка клеймов в пользователя, который содержит Jwt.
/// </summary>
/// <param name="user">Пользователь</param>
/// <param name="claims">Фабрика клеймов</param>
public sealed class JwtClaimsSessionUserSetup(JwtUser user, IJwtTokenClaimsFactory claims)
    : ISessionUserSetup
{
    public void Import() => user.Claims = claims.Create(user.User);
}
