using System.Security.Claims;
using Identity.Domain.Users.Aggregate;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Adapter.Jwt.Users;

public sealed class InitialJwtUser(User user) : JwtUser(user)
{
    private IEnumerable<Claim>? _claims;
    private RsaSecurityKey? _key;
    private SecurityTokenDescriptor? _descriptor;

    public override IEnumerable<Claim> Claims
    {
        get => _claims ?? throw new ArgumentException("Jwt user claims were not set.");
        set
        {
            if (_claims != null)
                throw new InvalidOperationException("Jwt user claims were already set.");
            _claims = value;
        }
    }

    public override RsaSecurityKey Key
    {
        get => _key ?? throw new ArgumentException("Rsa Security key was not set.");
        set
        {
            if (_key != null)
                throw new InvalidOperationException("Rsa security key was already set.");
            _key = value;
        }
    }

    public override SecurityTokenDescriptor Descriptor
    {
        set
        {
            if (_descriptor != null)
                throw new InvalidOperationException("Jwt user descriptor was already set.");
            _descriptor = value;
            _descriptor.Subject = new ClaimsIdentity(Claims);
            _descriptor.SigningCredentials = new SigningCredentials(
                Key,
                SecurityAlgorithms.RsaSha256
            );
        }
        get =>
            _descriptor ?? throw new InvalidOperationException("Jwt user descriptor was not set.");
    }

    public override IJwtUser Create()
    {
        JsonWebTokenHandler handler = new();
        string token = handler.CreateToken(_descriptor);
        return new SetJwtUser(token, user);
    }

    public override string ReadToken() =>
        throw new InvalidOperationException("Jwt user token was not created.");
}
