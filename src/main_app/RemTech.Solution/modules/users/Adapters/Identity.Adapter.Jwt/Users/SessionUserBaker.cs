using Identity.Adapter.Jwt.Claims;
using Identity.Adapter.Jwt.Security;
using Identity.Adapter.Jwt.Setups;
using Identity.Domain.Users.Aggregate;
using Identity.Jwt.Port;

namespace Identity.Adapter.Jwt.Users;

public sealed class SessionUserBaker(
    IJwtUserDescriptor descriptor,
    IRsaSecurityTokenPairStorage keys,
    IJwtTokenClaimsFactory claims
) : ISessionUserBaker
{
    public async Task<ISessionUserSetup> Bake(User user)
    {
        JwtUser setup = new InitialJwtUser(user);
        await SetupKeys(setup);
        SetupClaims(setup);
        SetupDescriptor(setup);
        setup.Create();
        return setup;
    }

    private async Task SetupKeys(JwtUser user)
    {
        await new JwtSecurityKeySessionUserSetup(user, keys).Import();
    }

    private void SetupClaims(JwtUser user)
    {
        new JwtClaimsSessionUserSetup(user, claims).Import();
    }

    private void SetupDescriptor(JwtUser user)
    {
        new JwtSecurityKeyDescriptorUserSetup(user, descriptor).Import();
    }
}
