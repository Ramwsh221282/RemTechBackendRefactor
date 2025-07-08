using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class PgCachingParsers(IParsersCache cache, IParsers origin) : IParsers
{
    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        MaybeBag<IParser> fromCache = await cache.Get(name);
        return fromCache.Any() ? fromCache.Take().Success() : await origin.Find(name, ct);
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        MaybeBag<IParser> fromCache = await cache.Get(new ParserCacheKey(id));
        return fromCache.Any() ? fromCache.Take().Success() : await origin.Find(id, ct);
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        MaybeBag<IParser> fromCache = await cache.Get(type, domain);
        return fromCache.Any() ? fromCache.Take().Success() : await origin.Find(type, domain, ct);
    }

    public async Task<Status> Add(IParser parser, CancellationToken ct = default)
    {
        Status adding = await origin.Add(parser, ct);
        if (adding.IsSuccess)
            await cache.Invalidate(new ParserCacheJson(parser));
        return adding;
    }

    public void Dispose()
    {
        origin.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return origin.DisposeAsync();
    }
}
