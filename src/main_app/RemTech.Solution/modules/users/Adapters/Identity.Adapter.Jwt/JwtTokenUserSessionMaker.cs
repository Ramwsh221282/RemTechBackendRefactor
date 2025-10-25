using System.Security.Claims;
using System.Text.Json;
using Identity.Adapter.Jwt.Claims;
using Identity.Adapter.Jwt.Security;
using Identity.Domain.Sessions;
using Identity.Domain.Sessions.Ports;
using Identity.Domain.Users.Aggregate;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.Shared.Configuration.Options;

namespace Identity.Adapter.Jwt;

public sealed class JwtTokenUserSessionMaker(
    IJwtTokenClaimsFactory claimsFactory,
    IRsaSecurityTokenPairStorage keysStorage,
    IOptions<FrontendOptions> frontend
) : IUserSessionMaker
{
    public async Task<UserSession> Create(User user)
    {
        await keysStorage.Generate();

        var key = await keysStorage.Get();
        var claims = CreateClaims(user);
        var descriptor = CreateDescriptor(key, claims);

        return new UserSession(CreateJwtToken(descriptor), CreateRefreshTokenInfo());
    }

    private IEnumerable<Claim> CreateClaims(User user)
    {
        return claimsFactory.Create(user);
    }

    private SecurityTokenDescriptor CreateDescriptor(RsaSecurityKey key, IEnumerable<Claim> claims)
    {
        return new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(key.Rsa),
                SecurityAlgorithms.RsaSha256
            ),
            Issuer = "rem-tech-agg",
            Expires = DateTime.UtcNow.AddMinutes(3),
            Audience = frontend.Value.FrontendUrl,
        };
    }

    private UserSessionInfo CreateJwtToken(SecurityTokenDescriptor descriptor)
    {
        JsonWebTokenHandler handler = new();
        string token = handler.CreateToken(descriptor);
        return new UserSessionInfo(token);
    }

    private UserSessionInfo CreateRefreshTokenInfo()
    {
        DateTime expires = DateTime.UtcNow.AddHours(12);
        string token = Guid.NewGuid().ToString();
        string type = "refresh_token";

        var structure = new { type, details = new { token, expires } };
        string content = JsonSerializer.Serialize(structure);

        return new UserSessionInfo(content);
    }
}
