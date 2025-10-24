using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Adapter.Jwt.Users;

public interface IJwtUser
{
    public IEnumerable<Claim> Claims { get; set; }
    public RsaSecurityKey Key { get; set; }
    public SecurityTokenDescriptor Descriptor { set; }
    public IJwtUser Create();
    public string ReadToken();
}
