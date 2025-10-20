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
        services.AddSingleton<ItemCleanedMessagePublisher>();
    }
}
