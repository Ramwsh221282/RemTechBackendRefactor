using Microsoft.Extensions.DependencyInjection;

namespace Cleaners.Adapter.Outbox;

public static class CleanersOutboxDependencyInjection
{
    public static void AddCleanersOutboxProcessor(this IServiceCollection services)
    {
        services.AddHostedService<CleanersOutboxProcessor>();
    }
}
