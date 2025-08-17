using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Parsing.Cache;

public sealed class DisabledTrackerConfiguration(string hostName)
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine($"Cache host: {hostName}");
        services.AddSingleton(ConnectionMultiplexer.Connect(hostName));
        services.AddSingleton<IDisabledScraperTracker, DisabledScraperTracker>();
    }
}
