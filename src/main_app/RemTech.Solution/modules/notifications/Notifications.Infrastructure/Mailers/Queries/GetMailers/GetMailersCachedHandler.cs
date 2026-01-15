using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

public sealed class GetMailersCachedHandler(
    HybridCache cache,
    IQueryHandler<GetMailersQuery, IEnumerable<MailerResponse>> inner
) : IQueryExecutorWithCache<GetMailersQuery, IEnumerable<MailerResponse>>
{
    public async Task<IEnumerable<MailerResponse>> ExecuteWithCache(
        GetMailersQuery query,
        CancellationToken ct = default
    )
    {
        string cacheKey = "mailers_array";
        return await cache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken => await inner.Handle(query, cancellationToken),
            cancellationToken: ct
        );
    }
}
