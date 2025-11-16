using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using RemTech.ContainedItems.Module.BackgroundJobs.ListenCleanedItemsMessage;
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
        services.AddHostedService<ItemCleanedMessageListener>();
    }
}
