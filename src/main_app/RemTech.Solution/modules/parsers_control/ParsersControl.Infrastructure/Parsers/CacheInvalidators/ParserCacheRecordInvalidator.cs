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

	public Task Invalidate(SubscribedParserId id, CancellationToken ct = default) => Invalidate(id.Value, ct);

	public Task Invalidate(SubscribedParser parser, CancellationToken ct = default) => Invalidate(parser.Id.Value, ct);
}
