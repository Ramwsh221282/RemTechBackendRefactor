using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
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
        await new RedisUpdatedParsersArray(_parsersArrayKey, json, array).Invalidate(engine);
    }

    public Task<MaybeBag<IParser>> Get(ParserCacheKey key)
    {
        return new MaybeSingleParserFromCache(key).Read(this);
    }

    public Task<MaybeBag<IParser>> Get(Name name)
    {
        return new MaybeSingleParserFromCacheByName(name).Read(this);
    }

    public Task<MaybeBag<IParser>> Get(ParsingType type, NotEmptyString domain)
    {
        return new MaybeSingleParserFromCacheByTypeAndDomain(type, domain).Read(this);
    }

    public async Task<IParser[]> Get()
    {
        RedisParsersCachedArray array = await GetArray();
        string[] jsons = array.Copy();
        return [.. jsons.Select(p => new SingleParserFromCache(p).Read())];
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
