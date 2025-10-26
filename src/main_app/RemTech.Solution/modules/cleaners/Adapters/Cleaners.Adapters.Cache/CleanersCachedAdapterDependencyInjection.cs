using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Cleaners.Adapters.Cache;

public static class CleanersCachedAdapterDependencyInjection
{
    public static void AddCachedCleaners(this IServiceCollection services)
    {
        services.AddSingleton<ICleanersCachedStorage, CleanersCachedStorage>();
    }
}
