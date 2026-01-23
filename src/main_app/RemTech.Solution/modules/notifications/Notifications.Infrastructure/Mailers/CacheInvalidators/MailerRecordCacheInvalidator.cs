using Microsoft.Extensions.Caching.Hybrid;

namespace Notifications.Infrastructure.Mailers.CacheInvalidators;

public sealed class MailerRecordCacheInvalidator(HybridCache cache)
{
	public async Task Invalidate(Guid id, CancellationToken ct = default)
	{
		string key = $"mailer_instance_{id}";
		await cache.RemoveAsync(key, ct);
	}
}
