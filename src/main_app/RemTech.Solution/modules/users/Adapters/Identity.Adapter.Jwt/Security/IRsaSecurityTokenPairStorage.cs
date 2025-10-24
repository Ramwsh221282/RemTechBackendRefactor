using Microsoft.IdentityModel.Tokens;

namespace Identity.Adapter.Jwt.Security;

public interface IRsaSecurityTokenPairStorage
{
    Task Generate();
    Task<RsaSecurityKey> Get();
}
