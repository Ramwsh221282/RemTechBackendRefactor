using Mailing.Domain.Postmans;

namespace Mailing.Adapters.Cache.Postmans;

internal sealed class CachedPostman(PostmansCache cache, IPostman Postman) : PostmanEnvelope(Postman)
{
    public async Task Save() => await cache.Add(this);

    public async Task<bool> HasUniqueEmail() => !(await cache.Contains(this));
}