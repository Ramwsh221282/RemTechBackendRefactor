using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers;

public sealed class RedisParsers(RedisCacheEngine engine) : IParsersCache
{
    private const string _parsersArrayKey = "rem_tech_parsers_array";

    public async Task Invalidate(ParserCacheJson json)
    {
        RedisParsersCachedArray array = await GetArray();
        if (!array.Any())
        {
            await new RedisInitialParsersArray(_parsersArrayKey, json).Invalidate(engine);
            return;
        }
        RedisParsersCachedArray current = await GetArray();
        await new RedisUpdatedParsersArray(_parsersArrayKey, json, current).Invalidate(engine);
    }

    public async Task<MaybeBag<IParser>> Get(ParserCacheKey key)
    {
        RedisParsersCachedArray array = await GetArray();
        return new MaybeSingleParserFromCache(array, key).Read();
    }

    public async Task<IParser[]> Get()
    {
        RedisParsersCachedArray array = await GetArray();
        IParser[] parsers = new IParser[array.Length()];
        string[] jsons = array.Copy();
        for (int i = 0; i < array.Length(); i++)
        {
            string json = jsons[i];
            IParser parser = new SingleParserFromCache(json).Read();
            parsers[i] = parser;
        }
        return parsers;
    }

    public async Task<RedisParsersCachedArray> GetArray()
    {
        return await new MaybeRedisParsersCachedArray(_parsersArrayKey).Read(engine);
    }

    public async Task DropArray()
    {
        await new RedisDeletedParsersArray(_parsersArrayKey).Invalidate(engine);
    }
}
