using Microsoft.Extensions.Caching.Hybrid;

namespace Notifications.Infrastructure.Mailers.CacheInvalidators;

public sealed class MailerArrayCacheInvalidator(HybridCache cache)
{
    public async Task Invalidate(CancellationToken ct = default)
    {
        string key = "mailers_array";
        await cache.RemoveAsync(key, ct);
    }
}