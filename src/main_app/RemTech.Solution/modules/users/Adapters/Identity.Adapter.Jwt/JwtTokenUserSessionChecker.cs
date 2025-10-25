using Identity.Adapter.Jwt.Security;
using Identity.Domain.Sessions;
using Identity.Domain.Sessions.Ports;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.Shared.Configuration.Options;

namespace Identity.Adapter.Jwt;

public sealed class JwtTokenUserSessionChecker(
    IRsaSecurityTokenPairStorage keys,
    IOptions<FrontendOptions> frontend
) : IUserSessionChecker
{
    public async Task<bool> IsValid(UserSession session)
    {
        var parameters = await CreateValidationParameters();
        return await Validate(session, parameters);
    }

    private async Task<bool> Validate(UserSession session, TokenValidationParameters parameters)
    {
        JsonWebTokenHandler handler = new();
        try
        {
            string token = session.AccessToken.Token;
            TokenValidationResult result = await handler.ValidateTokenAsync(token, parameters);
            return result.IsValid;
        }
        catch
        {
            return false;
        }
    }

    private async Task<TokenValidationParameters> CreateValidationParameters()
    {
        var key = await keys.Get();
        return new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(key.Rsa.ExportParameters(false)),
            ValidateIssuer = true,
            ValidIssuer = "rem-tech-agg",
            ValidateAudience = true,
            ValidAudience = frontend.Value.FrontendUrl,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    }
}
