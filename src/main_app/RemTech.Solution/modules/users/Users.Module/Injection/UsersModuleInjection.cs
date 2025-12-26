using Microsoft.Extensions.DependencyInjection;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail;
using Users.Module.Features.RolesSeeding;
using Users.Module.Public;

namespace Users.Module.Injection;

public static class UsersModuleInjection
{
    public static void InjectUsersModule(this IServiceCollection services)
    {
        services.AddHostedService<RolesSeedingOnStartup>();
        services.AddSingleton(new StringHash());
        services.AddSingleton(new SecurityKeySource());
        services.AddTransient<AdminOrRootAccessFilter>();
        services.AddSingleton<PrivelegedAccessVerify>();
        services.AddSingleton<ConfirmationEmailsCache>();
    }
}
