using Identity.Adapter.Jwt.Users;

namespace Identity.Adapter.Jwt.Security;

public interface IJwtUserDescriptor
{
    void Descript(JwtUser user);
}
