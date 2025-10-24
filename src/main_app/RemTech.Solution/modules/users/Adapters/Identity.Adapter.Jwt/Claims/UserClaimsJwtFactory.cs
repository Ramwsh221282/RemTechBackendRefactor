using System.Security.Claims;
using Identity.Domain.Roles.ValueObjects;
using Identity.Domain.Users.Aggregate;

namespace Identity.Adapter.Jwt.Claims;

public sealed class UserClaimsJwtFactory : IJwtTokenClaimsFactory
{
    public IEnumerable<Claim> Create(User user)
    {
        IEnumerable<Claim> claims = [.. CreateRoleClaims(user), .. CreateProfileClaims(user)];
        return claims;
    }

    private IEnumerable<Claim> CreateRoleClaims(User user)
    {
        IEnumerable<RoleName> roles = user.Roles.Roles.Select(r => r.Name);
        IEnumerable<Claim> claims = roles.Select(r => new Claim("Role", r.Value));
        return claims;
    }

    private IEnumerable<Claim> CreateProfileClaims(User user)
    {
        Claim subject = new Claim("Subject", user.Id.Id.ToString());
        Claim verified = new Claim("Verified", user.Profile.EmailConfirmed.ToString());
        Claim name = new Claim("Name", user.Profile.Login.Name);
        Claim email = new Claim("Email", user.Profile.Email.Email);
        Claim tokenId = new Claim("TokenId", Guid.NewGuid().ToString());
        return [subject, verified, name, email, tokenId];
    }
}
