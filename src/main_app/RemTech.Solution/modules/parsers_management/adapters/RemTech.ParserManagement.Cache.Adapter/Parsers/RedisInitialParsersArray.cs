using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class RedisInitialParsersArray(string key, ParserCacheJson json)
{
    public async Task<RedisParsersCachedArray> Invalidate(RedisCacheEngine engine)
    {
        RedisParsersCachedArray array = new(json);
        await engine.Access().StringSetAsync(key, array.Serialized());
        return array;
    }
}