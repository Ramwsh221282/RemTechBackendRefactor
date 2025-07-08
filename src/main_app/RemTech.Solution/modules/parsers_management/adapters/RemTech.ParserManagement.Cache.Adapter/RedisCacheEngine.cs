using RemTech.ParserManagement.Cache.Adapter.Configuration;
using StackExchange.Redis;

namespace RemTech.ParserManagement.Cache.Adapter;

public sealed class RedisCacheEngine
{
    private readonly ConnectionMultiplexer _connection;

    public RedisCacheEngine(RedisConfiguration configuration)
    {
        _connection = ConnectionMultiplexer.Connect(configuration);
    }

    public IDatabase Access()
    {
        return _connection.GetDatabase();
    }
}
