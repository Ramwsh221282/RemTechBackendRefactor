using Identity.Domain.Accounts.Models;
using Identity.Domain.Tokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Jwt;

public interface IJwtTokenManager
{
    AccessToken GenerateToken(Account account);
    Task<Result<TokenValidationResult>> GetValidToken(string jwtToken);
}