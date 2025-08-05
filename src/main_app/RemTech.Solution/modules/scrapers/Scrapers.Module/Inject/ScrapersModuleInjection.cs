using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using Scrapers.Module.Features.CreateNewParser.Inject;

namespace Scrapers.Module.Inject;

public static class ScrapersModuleInjection
{
    public static void InjectScrapersModule(this IServiceCollection services)
    {
        CreateNewParserInjection.Inject(services);
    }

    public static void UpScrapersModuleDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(ScrapersModuleInjection).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create scrapers database.");
    }
}
