using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

public sealed class ParsersCache(IParsersCache cache) : IParsersCache
{
    public Task Invalidate(ParserCacheJson json)
    {
        return cache.Invalidate(json);
    }

    public Task<MaybeBag<IParser>> Get(ParserCacheKey key)
    {
        return cache.Get(key);
    }

    public Task<IParser[]> Get()
    {
        return cache.Get();
    }
}
