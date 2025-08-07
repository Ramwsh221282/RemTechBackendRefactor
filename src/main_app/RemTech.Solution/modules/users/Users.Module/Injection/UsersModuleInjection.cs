using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Users.Module.CommonAbstractions;
using Users.Module.Features.AuthenticateUser.Endpoint;
using Users.Module.Features.AuthenticateUser.Jwt;
using Users.Module.Features.RegisteringUser;
using Users.Module.Features.RegisteringUser.Endpoint;
using Users.Module.Options;

namespace Users.Module.Injection;

public static class UsersModuleInjection
{
    public static void InjectUsersModule(this IServiceCollection services)
    {
        services.AddSingleton(new StringHash());
        services.AddSingleton(new SecurityKeySource());
    }

    public static void MapUsersModuleEndpoints(this WebApplication app)
    {
        RouteGroupBuilder builder = app.MapGroup("api/users").RequireCors("FRONTEND");
        RegisterUserFeatureEndpoint.Map(builder);
        AuthenticateUserFeatureEndpoint.Map(builder);
    }

    public static void UpUsersModuleDatabase(string connectionString)
    {
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
