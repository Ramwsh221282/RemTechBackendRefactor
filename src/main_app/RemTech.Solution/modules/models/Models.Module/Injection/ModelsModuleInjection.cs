using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using Models.Module.Features.AddModelsOnStartup;
using Models.Module.Public;

namespace Models.Module.Injection;

public static class ModelsModuleInjection
{
    public static void InjectModelsModule(this IServiceCollection services)
    {
        services.AddHostedService<CsvModelsSeeding>();
        services.AddSingleton<IModelPublicApi, ModelPublicApi>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(CsvModelsSeeding).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create models module database.");
    }
}
