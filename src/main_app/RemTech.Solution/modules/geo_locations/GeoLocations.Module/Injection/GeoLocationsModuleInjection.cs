using DbUp;
using DbUp.Engine;
using GeoLocations.Module.Features.Querying;
using GeoLocations.Module.OnStartup;
using Microsoft.Extensions.DependencyInjection;

namespace GeoLocations.Module.Injection;

public static class GeoLocationsModuleInjection
{
    public static void InjectLocationsModule(this IServiceCollection services)
    {
        services.AddHostedService<LocationsSeeding>();
        services.AddSingleton<IGeoLocationQueryService, GeoLocationsQueryService>();
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(LocationsSeeding).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create locations module database.");
    }
}
