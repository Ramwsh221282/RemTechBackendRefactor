using Identity.Adapter.Jwt.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RemTech.Shared.Configuration.Options;

namespace Identity.Adapter.Jwt.Security;

public sealed class JwtUserDescriptor(
    string issuer,
    DateTime expires,
    IOptions<FrontendOptions> frontend
) : IJwtUserDescriptor
{
    private readonly string _audienceUrl = frontend.Value.FrontendUrl;

    public void Descript(JwtUser user)
    {
        user.Descriptor = new SecurityTokenDescriptor()
        {
            Issuer = issuer,
            Expires = expires,
            Audience = _audienceUrl,
        };
    }
}
