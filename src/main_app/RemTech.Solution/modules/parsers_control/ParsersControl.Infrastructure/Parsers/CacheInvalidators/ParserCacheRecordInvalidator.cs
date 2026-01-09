using Microsoft.Extensions.Caching.Hybrid;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Infrastructure.Parsers.CacheInvalidators;

public sealed class ParserCacheRecordInvalidator(HybridCache cache)
{
    public async Task Invalidate(Guid id, CancellationToken ct = default)
    {
        string cacheKey = $"get_parser_{id}";
        await cache.RemoveAsync(cacheKey, cancellationToken: ct);
    }
    
    public async Task Invalidate(SubscribedParserId id, CancellationToken ct = default) =>
        await Invalidate(id.Value, ct);
    
    public async Task Invalidate(SubscribedParser parser, CancellationToken ct = default) =>
        await Invalidate(parser.Id.Value, ct);
}