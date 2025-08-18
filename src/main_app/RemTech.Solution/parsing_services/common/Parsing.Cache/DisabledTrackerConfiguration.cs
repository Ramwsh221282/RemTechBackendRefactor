using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Parsing.Cache;

public sealed class DisabledTrackerConfiguration(string hostName)
{
    public void Register(IServiceCollection services)
    {
        Console.WriteLine($"Cache host: {hostName}");
        ConfigurationOptions opts = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            AsyncTimeout = 5000,
            AllowAdmin = true,
            EndPoints = { hostName },
            ConnectTimeout = 5000,
            ConnectRetry = 3,
        };
        ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(opts);
        services.AddSingleton(multiplexer);
        services.AddSingleton<IDisabledScraperTracker, DisabledScraperTracker>();
    }
}
