using Microsoft.IdentityModel.Tokens;

namespace Identity.Domain.Extensions;

public static class TokenValidationResultExtensions
{
    extension(TokenValidationResult result)
    {
        public Guid TokenId => Guid.Parse(result.Claims["http://schemas.microsoft.com/identity/claims/tenantid"].ToString()!);
    }
}