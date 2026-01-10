using Microsoft.Extensions.Caching.Hybrid;
using Notifications.Infrastructure.Mailers.Queries.GetMailers;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailer;

public sealed class GetMailerCachedHandler(
    HybridCache cache,
    IQueryHandler<GetMailerQuery, MailerResponse?> inner)
    : IQueryHandler<GetMailerQuery, MailerResponse?>
{
    public async Task<MailerResponse?> Handle(GetMailerQuery query, CancellationToken ct = default)
    {
        string key = $"mailer_instance_{query.Id}";
        
        MailerResponse? cached = await cache.GetOrCreateAsync(
            key,
            async cancellationToken => await inner.Handle(query, cancellationToken),
            cancellationToken: ct);
        
        return cached;
    }
}