using Brands.Module.Features.AddBrandsOnStartup;
using Brands.Module.Public;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Brands.Module.Injection;

public static class BrandsModuleInjection
{
    public static void InjectBrandsModule(this IServiceCollection services)
    {
        services.AddHostedService<SeedingBrandsOnStartup>();
        services.AddSingleton<IBrandsPublicApi, BrandsPublicApi>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(SeedingBrandsOnStartup).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create brands module database.");
    }
}
