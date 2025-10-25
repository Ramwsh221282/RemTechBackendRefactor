using Identity.Adapter.Jwt.Claims;
using Identity.Adapter.Jwt.Security;
using Identity.Domain.Sessions;
using Identity.Domain.Sessions.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Adapter.Jwt;

public static class IdentityJwtExtensions
{
    public static void AddIdentityJwt(this IServiceCollection services)
    {
        services.AddSingleton<IUserSessionsStorage, JwtTokenUserSessionsStorage>();
        services.AddSingleton<IUserSessionMaker, JwtTokenUserSessionMaker>();
        services.AddSingleton<IUserSessionRefresher, JwtTokenUserSessionRefresher>();
        services.AddSingleton<IUserSessionChecker, JwtTokenUserSessionChecker>();
        services.AddSingleton<IRsaSecurityTokenPairStorage, RsaSecurityKeyPairStorage>();
        services.AddSingleton<IJwtTokenClaimsFactory, UserClaimsJwtFactory>();
        services.AddScoped<UserSessionsService>();
    }
}
