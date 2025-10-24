using Identity.Adapter.Jwt.Security;
using Identity.Adapter.Jwt.Users;
using Identity.Jwt.Port;

namespace Identity.Adapter.Jwt.Setups;

/// <summary>
/// Подписка пользователя с JWT
/// </summary>
/// <param name="user">Пользователь</param>
/// <param name="descriptor">Подписыватель</param>
public sealed class JwtSecurityKeyDescriptorUserSetup(JwtUser user, IJwtUserDescriptor descriptor)
    : ISessionUserSetup
{
    public void Import() => descriptor.Descript(user);
}
