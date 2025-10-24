using System.Security.Claims;
using Identity.Domain.Users.Aggregate;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Adapter.Jwt.Users;

public sealed class SetJwtUser(string token, User user) : JwtUser(user)
{
    private const string ErrorOnAccess = "Jwt user already created.";

    public override IEnumerable<Claim> Claims
    {
        get => throw new InvalidOperationException(ErrorOnAccess);
        set => throw new InvalidOperationException(ErrorOnAccess);
    }

    public override RsaSecurityKey Key
    {
        get => throw new InvalidOperationException(ErrorOnAccess);
        set => throw new InvalidOperationException(ErrorOnAccess);
    }

    public override SecurityTokenDescriptor Descriptor
    {
        get => throw new InvalidOperationException(ErrorOnAccess);
        set => throw new InvalidOperationException(ErrorOnAccess);
    }

    public override IJwtUser Create() =>
        throw new InvalidOperationException("Jwt user already created.");

    public override string ReadToken() => token;
}
