using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Common;

public sealed class JwtTokenManager(IOptions<JwtOptions> options) : IJwtTokenManager
{
    private TokenValidationParameters? _validationParameters;
    private JwtOptions Options { get; } = options.Value;

    public AccessToken GenerateToken(Account account)
    {
        string token = CreateToken(CreateTokenDescriptor(account));
        return CreateStructuredAccessToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid accountId)
    {
        string token = CreateRefreshToken();
        (long expiresAt, long createdAt) = ReadTokenLifeTime(token);
        return RefreshToken.CreateNew(accountId, expiresAt, createdAt);
    }
    
    public AccessToken ReadToken(string tokenString) =>
        CreateStructuredAccessToken(tokenString);
    
    public async Task<Result<TokenValidationResult>> GetValidToken(string jwtToken)
    {
        try
        {
            JwtSecurityTokenHandler handler = new();
            if (!handler.CanValidateToken) 
                return Error.Unauthorized("Invalid token");
            if (!handler.CanReadToken(jwtToken))
                return Error.Unauthorized("Invalid token");
            
            TokenValidationParameters parameters = CreateValidationParameters();
            TokenValidationResult validationResult = await handler.ValidateTokenAsync(jwtToken, parameters);
            if (!validationResult.IsValid)
                return Error.Unauthorized("Invalid token");
            
            return validationResult;
        }
        catch
        {
            return Error.Unauthorized("Invalid token");
        }
    }

    private (long expiresAt, long createdAt) ReadTokenLifeTime(string tokenString)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(tokenString);
        JwtPayload payload = token.Payload;
        return (
            long.Parse(payload["exp"].ToString()!),
            long.Parse(payload["nbf"].ToString()!) // nbf is used to determine the start time of the token.
        );
    }
    
    private AccessToken CreateStructuredAccessToken(string tokenString)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(tokenString);
        JwtPayload payload = token.Payload;
        return new AccessToken()
        {
            RawToken = tokenString,
            TokenId = Guid.Parse(payload["tid"].ToString()!),
            ExpiresAt = long.Parse(payload["exp"].ToString()!),
            RawPermissionsString = payload["permissions"].ToString()!,
            Permissions = payload["permissions"].ToString()!.Split(','),
            Email = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"].ToString()!,
            Login = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"].ToString()!,
            UserId = Guid.Parse(payload["id"].ToString()!),
            CreatedAt = long.Parse(payload["nbf"].ToString()!) // nbf is used to determine the start time of the token.
        };
    }
    
    private TokenValidationParameters CreateValidationParameters()
    {
        if (_validationParameters is not null) return _validationParameters;
        _validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Options.SecretKey)),
            ValidAudience = Options.Audience,
            ValidIssuer = Options.Issuer
        };
        return _validationParameters;
    }

    private string CreateToken(SecurityTokenDescriptor descriptor)
    {
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: Options.Issuer,
            audience: Options.Audience,
            claims: descriptor.Subject.Claims,
            notBefore: DateTime.UtcNow,
            expires: descriptor.Expires,
            signingCredentials: descriptor.SigningCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateRefreshToken(int days = 7)
    {
        JwtSecurityToken token = new JwtSecurityToken(
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(days)
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private SecurityTokenDescriptor CreateTokenDescriptor(Account account)
    {
        SecurityTokenDescriptor tokenDescriptor = new();
        tokenDescriptor.Subject = CreateClaims(account);
        tokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(5);
        tokenDescriptor.SigningCredentials = CreateSigningCredentials();
        return tokenDescriptor;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Options.SecretKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
    
    private ClaimsIdentity CreateClaims(Account account)
    {
        List<Claim> claims = [];
        claims.Add(new Claim(ClaimTypes.Name, account.Login.Value));
        claims.Add(new Claim(ClaimTypes.Email, account.Email.Value));
        claims.Add(new Claim("id", account.Id.Value.ToString()));
        claims.Add(new Claim("tid", Guid.NewGuid().ToString()));
        claims.Add(new Claim("permissions", string.Join(",", account.PermissionsList.Select(p => p.Name.Value))));
        return new ClaimsIdentity(claims);
    }
}