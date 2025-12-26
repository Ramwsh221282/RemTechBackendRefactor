using Identity.Adapter.Auth.Middleware;
using Identity.Adapter.Jwt;
using Identity.Adapter.Notifier;
using Identity.Adapter.PasswordManager;
using Identity.Adapter.Storage.DependencyInjection;
using Identity.Domain.DependencyInjection;
using Shared.WebApi;

namespace Identity.WebApi;

public static class IdentityModuleInjection
{
    public static void AddIdentityModule(this IServiceCollection services)
    {
        services.InjectIdentityDomain();
        services.InjectBcryptPasswordManager();
        services.InjectIdentityStorageAdapter();
        services.AddUsersNotifier();
        services.AddIdentityJwt();
        services.AddSingleton<IRoleAccessChecker, RoleAccessChecker>();
    }
}
