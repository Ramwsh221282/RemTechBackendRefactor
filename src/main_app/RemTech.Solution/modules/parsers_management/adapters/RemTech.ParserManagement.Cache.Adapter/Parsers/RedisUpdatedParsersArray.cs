using System.Diagnostics;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
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
        string[] copies = current.Copy();
        int idx = TryGetIndex(copies);
        if (idx >= 0)
            copies[idx] = json.Json();
        else
            copies = [.. copies, json.Json()];
        RedisParsersCachedArray updated = new(copies);
        await engine.Access().StringSetAsync(key, updated.Serialized());
        return updated;
    }

    public int TryGetIndex(string[] copies)
    {
        IParser converted = new SingleParserFromCache(json.Json()).Read();
        for (int i = 0; i < copies.Length; i++)
        {
            string cacheJson = copies[i];
            IParser related = new SingleParserFromCache(cacheJson).Read();
            if (converted.Identification().ReadId().Equals(related.Identification().ReadId()))
                return i;
        }
        return -1;
    }
}
