using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Users.Module.CommonAbstractions;

namespace Users.Module.Models;

internal sealed class UserJwtSource(
    Guid userId,
    string name,
    string email,
    string password,
    string role
)
{
    public UserJwt Provide(SecurityKeySource securityKey, Guid? tokenId = null)
    {
        Guid actualToken = tokenId ?? Guid.NewGuid();
        DateTime expirationDate = DateTime.Now.AddMinutes(5);
        DateTime refreshExpirationDate = DateTime.Now.AddDays(7);
        Claim[] claims =
        [
            new("userId", userId.ToString()),
            new("tokenId", Guid.NewGuid().ToString()),
        ];
        SigningCredentials credentials = securityKey.Credentials();
        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: credentials,
            expires: expirationDate
        );
        JwtSecurityToken refreshToken = new JwtSecurityToken(
            signingCredentials: credentials,
            expires: refreshExpirationDate
        );
        string refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);
        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new UserJwt(
            userId,
            actualToken,
            name,
            password,
            email,
            role,
            tokenString,
            refreshTokenString
        );
    }
}
