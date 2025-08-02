using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Users.Module.CommonAbstractions;
using Users.Module.Features.RegisteringUser;
using Users.Module.Features.RegisteringUser.Inject;
using Users.Module.Options;

namespace Users.Module.Injection;

public static class UsersModuleInjection
{
    public static void InjectUsersModule(
        this IServiceCollection services,
        UsersModuleOptions options
    )
    {
        services.AddSingleton(new PgConnectionSource(options));
        services.AddSingleton(new StringHash());
    }

    public static void MapUsersModuleEndpoints(this WebApplication app)
    {
        RouteGroupBuilder builder = app.MapGroup("api/users").RequireCors("FRONTEND");
        RegisterUserFeatureInject.Inject(builder);
    }

    public static void UpUsersModulePersistance(this UsersModuleOptions options)
    {
        string connectionString = options.Database.ToConnectionString();
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(IUserToRegister).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create users module database.");
    }
}
