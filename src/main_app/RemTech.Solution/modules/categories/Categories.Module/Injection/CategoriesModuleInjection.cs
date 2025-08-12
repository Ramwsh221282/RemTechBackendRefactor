using Categories.Module.Features.AddCategoriesOnStartup;
using Categories.Module.Features.QueryCategories;
using Categories.Module.Features.QueryCategoriesAmount;
using Categories.Module.Features.QueryPopularCategories;
using Categories.Module.Public;
using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Categories.Module.Injection;

public static class CategoriesModuleInjection
{
    public static void InjectCategoriesModule(this IServiceCollection services)
    {
        services.AddHostedService<SeedingCategoriesOnStartup>();
        services.AddSingleton<ICategoryPublicApi, CategoryPublicApi>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(SeedingCategoriesOnStartup).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create categories module database.");
    }

    public static void MapCategoriesEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/categories");
        QueryPopularCategoriesEndpoint.Map(group);
        QueryCategoriesEndpoint.Map(group);
        QueryCategoriesAmountEndpoint.Map(group);
    }
}
