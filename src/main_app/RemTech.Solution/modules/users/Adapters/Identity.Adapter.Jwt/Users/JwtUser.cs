using System.Security.Claims;
using Identity.Domain.Users.Aggregate;
using Identity.Jwt.Port;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Adapter.Jwt.Users;

public abstract class JwtUser(User user) : IJwtUser, ISessionUserSetup
{
    public readonly User User = user;
    public abstract IEnumerable<Claim> Claims { get; set; }
    public abstract RsaSecurityKey Key { get; set; }
    public abstract SecurityTokenDescriptor Descriptor { get; set; }
    public abstract IJwtUser Create();
    public abstract string ReadToken();

    public void Import() { }
}
