using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.BackgroundService;
using Shared.Infrastructure.Module.Postgres;

namespace RemTech.Vehicles.Module.Injection;

public sealed class ParsedAdvertisementsStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public ParsedAdvertisementsStorageUpper(IOptions<DatabaseOptions> options) =>
        _options = options;

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();

        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                typeof(BackgroundJobTransportAdvertisementSinking).Assembly
            )
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();

        return !result.Successful
            ? throw new ApplicationException("Failed to create parsers management database.")
            : Task.CompletedTask;
    }
}

public static class ParsedAdvertisementsInjection
{
    public static void InjectVehiclesModule(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundJobTransportAdvertisementSinking>();
    }
}
