using Identity.Domain.Accounts.Models;
using Identity.Domain.Tokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Jwt;

public interface IJwtTokenManager
{
    public AccessToken GenerateToken(Account account);
    public AccessToken ReadToken(string tokenString);
    public Task<Result<TokenValidationResult>> GetValidToken(string jwtToken);
    public RefreshToken GenerateRefreshToken(Guid accountId);
}
