using System.Threading.Channels;
using DbUp;
using DbUp.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RemTech.ContainedItems.Module.Features.AddFirstCleaner;
using RemTech.ContainedItems.Module.Features.GetContainedVehiclesAmount;
using RemTech.ContainedItems.Module.Features.MessageBus;
using RemTech.ContainedItems.Module.Features.QueryRecentContainedItems;
using RemTech.ContainedItems.Module.Features.QueryRecentContainedItemsCount;

namespace RemTech.ContainedItems.Module.Injection;

public static class ContainedItemsModuleInjection
{
    public static void InjectContainedItemsModule(this IServiceCollection services)
    {
        services.AddHostedService<AddContainedItemsBus>();
        services.AddHostedService<AddFirstCleanerOnStartup>();
        services.AddSingleton<IAddContainedItemsPublisher, AddContainedItemsPublisher>();
        services.AddSingleton<Channel<AddContainedItemMessage>>(_ =>
            Channel.CreateUnbounded<AddContainedItemMessage>()
        );
    }

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(AddContainedItemsBus).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create contained items module database.");
    }
}
