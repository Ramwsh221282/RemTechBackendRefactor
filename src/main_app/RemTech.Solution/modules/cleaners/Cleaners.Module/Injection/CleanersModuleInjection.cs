using System.Threading.Channels;
using Cleaners.Module.BackgroundJobs.CleanItemsListening;
using Cleaners.Module.BackgroundJobs.FinishingListening;
using Cleaners.Module.Contracts.ItemCleaned;
using Cleaners.Module.OnStartup;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Cleaners.Module.Injection;

public static class CleanersModuleInjection
{
    public static void InjectCleanersModule(this IServiceCollection services)
    {
        services.AddHostedService<CreateFirstCleanerOnStartup>();
        services.AddHostedService<CleanItemBackgroundListener>();
        services.AddHostedService<FinishCleanerBackgroundListener>();
        services.AddSingleton(Channel.CreateUnbounded<ItemCleanedMessage>());
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(CreateFirstCleanerOnStartup).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create cleaners module database.");
    }
}
