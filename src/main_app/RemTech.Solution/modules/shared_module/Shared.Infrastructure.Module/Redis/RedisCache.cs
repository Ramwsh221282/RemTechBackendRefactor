using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using StackExchange.Redis;

namespace Shared.Infrastructure.Module.Redis;

public sealed class RedisCache
{
    private readonly ConnectionMultiplexer _multiplexer;

    public IDatabase Database => _multiplexer.GetDatabase();

    public RedisCache(IOptions<CacheOptions> options)
    {
        ConfigurationOptions opts = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            AsyncTimeout = 5000,
            AllowAdmin = true,
            EndPoints = { options.Value.Host },
            ConnectTimeout = 5000,
            ConnectRetry = 3,
        };

        _multiplexer = ConnectionMultiplexer.Connect(opts);
    }
}
