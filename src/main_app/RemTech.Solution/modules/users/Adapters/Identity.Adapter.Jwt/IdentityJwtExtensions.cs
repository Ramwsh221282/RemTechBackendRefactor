using Identity.Adapter.Jwt.Claims;
using Identity.Adapter.Jwt.Security;
using Identity.Adapter.Jwt.Users;
using Identity.Jwt.Port;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Adapter.Jwt;

public static class IdentityJwtExtensions
{
    public static void AddIdentityJwt(IServiceCollection services)
    {
        services.AddSingleton<IJwtUserDescriptor, JwtUserDescriptor>();
        services.AddSingleton<IRsaSecurityTokenPairStorage, RsaSecurityKeyPairStorage>();
        services.AddSingleton<IJwtTokenClaimsFactory, IJwtTokenClaimsFactory>();
        services.AddJwtUserBaker();
    }

    private static void AddJwtUserBaker(this IServiceCollection services)
    {
        services.AddSingleton<ISessionUserBaker, SessionUserBaker>(sp =>
        {
            var descriptor = sp.GetRequiredService<IJwtUserDescriptor>();
            var keys = sp.GetRequiredService<IRsaSecurityTokenPairStorage>();
            var claims = sp.GetRequiredService<IJwtTokenClaimsFactory>();
            var baker = new SessionUserBaker(descriptor, keys, claims);
            return baker;
        });
    }
}
