using Microsoft.Extensions.DependencyInjection;

namespace Cleaner.Adapter.Queues;

public static class CleanerQueueDependencyInjection
{
    public static void AddCleanerQueues(this IServiceCollection services)
    {
        services.AddHostedService<CleanerStateUpdatedQueue>();
        services.AddHostedService<CleanerWorkFinishedQueue>();
    }
}
