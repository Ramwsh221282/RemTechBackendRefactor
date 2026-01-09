using Microsoft.Extensions.Caching.Hybrid;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

namespace ParsersControl.Infrastructure.Parsers.CacheInvalidators;

public sealed class CachedParserArrayInvalidator(HybridCache cache)
{
    public async Task Invalidate(CancellationToken ct = default)
    {
        await cache.RemoveAsync(ParserCacheContants.Array, cancellationToken: ct);
    }
}