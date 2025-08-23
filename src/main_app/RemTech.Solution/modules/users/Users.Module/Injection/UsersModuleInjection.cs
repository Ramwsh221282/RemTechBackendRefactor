using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.CreatingNewAccount;
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

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(JwtUserResult).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create users module database.");
    }
}
