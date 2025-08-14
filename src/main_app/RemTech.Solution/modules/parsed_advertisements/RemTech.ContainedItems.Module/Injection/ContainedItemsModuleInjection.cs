using System.Threading.Channels;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.DependencyInjection;
using RemTech.ContainedItems.Module.Features.MessageBus;

namespace RemTech.ContainedItems.Module.Injection;

public static class ContainedItemsModuleInjection
{
    public static void InjectContainedItemsModule(this IServiceCollection services)
    {
        services.AddHostedService<AddContainedItemsBus>();
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
