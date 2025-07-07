using System.Diagnostics;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class RedisUpdatedParsersArray(
    string key,
    ParserCacheJson json,
    RedisParsersCachedArray current
)
{
    public async Task<RedisParsersCachedArray> Invalidate(RedisCacheEngine engine)
    {
        RedisParsersArrayIndex maybeIndex = new(current, json);
        int index = maybeIndex.Index();
        if (index < 0)
            throw new UnreachableException(
                "Не найден индекс для обновления массива кешированных парсеров."
            );
        RedisParsersCachedArray updated = new(current, json, index);
        await engine.Access().StringSetAsync(key, updated.Serialized());
        return updated;
    }
}
