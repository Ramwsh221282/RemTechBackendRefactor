using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Spares.Module.Features.QuerySpare;
using RemTech.Spares.Module.Features.QuerySpareTotals;
using RemTech.Spares.Module.Features.SinkSpare;

namespace RemTech.Spares.Module.Injection;

public static class SparesModuleInjection
{
    public static void InjectSparesModule(this IServiceCollection services)
    {
        services.AddHostedService<SpareSinkEntrance>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(SpareSinkEntrance).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create spares module database.");
    }
}
