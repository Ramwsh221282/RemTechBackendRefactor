using Identity.Adapter.Jwt.Security;
using Identity.Adapter.Jwt.Users;
using Identity.Jwt.Port;

namespace Identity.Adapter.Jwt.Setups;

/// <summary>
/// Установка приватного/публичного ключа в JWT пользователя.
/// </summary>
/// <param name="user">JWT пользователь</param>
/// <param name="keys">Источник ключей</param>
public sealed class JwtSecurityKeySessionUserSetup(JwtUser user, IRsaSecurityTokenPairStorage keys)
    : ISessionUserSetupAsync
{
    public async Task Import() => user.Key = await keys.Get();
}
