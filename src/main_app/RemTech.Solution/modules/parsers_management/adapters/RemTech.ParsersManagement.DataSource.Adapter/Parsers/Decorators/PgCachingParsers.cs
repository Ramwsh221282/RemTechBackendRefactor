using Npgsql;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class PgCachingParsers(NpgsqlDataSource source, IParsersCache cache, IParsers origin)
    : IParsers
{
    public async Task<Status<IParser>> Find(Name name, CancellationToken ct = default)
    {
        MaybeBag<IParser> fromCache = await cache.Get(name);
        if (fromCache.Any())
            return new PgCachedParser(
                cache,
                new PgParser(fromCache.Take(), await source.OpenConnectionAsync(ct))
            );
        Status<IParser> fromOrigin = await origin.Find(name, ct);
        if (fromOrigin.IsSuccess)
            return new PgCachedParser(
                cache,
                new PgParser(fromOrigin.Value, await source.OpenConnectionAsync(ct))
            );
        return fromOrigin.Error;
    }

    public async Task<Status<IParser>> Find(NotEmptyGuid id, CancellationToken ct = default)
    {
        MaybeBag<IParser> fromCache = await cache.Get(new ParserCacheKey(id));
        if (fromCache.Any())
            return new PgCachedParser(
                cache,
                new PgParser(fromCache.Take(), await source.OpenConnectionAsync(ct))
            );
        Status<IParser> fromOrigin = await origin.Find(id, ct);
        if (fromOrigin.IsSuccess)
            return new PgCachedParser(
                cache,
                new PgParser(fromOrigin.Value, await source.OpenConnectionAsync(ct))
            );
        return fromOrigin.Error;
    }

    public async Task<Status<IParser>> Find(
        ParsingType type,
        NotEmptyString domain,
        CancellationToken ct = default
    )
    {
        MaybeBag<IParser> fromCache = await cache.Get(type, domain);
        if (fromCache.Any())
            return new PgCachedParser(
                cache,
                new PgParser(fromCache.Take(), await source.OpenConnectionAsync(ct))
            );
        Status<IParser> fromOrigin = await origin.Find(type, domain, ct);
        if (fromOrigin.IsSuccess)
            return new PgCachedParser(
                cache,
                new PgParser(fromOrigin.Value, await source.OpenConnectionAsync(ct))
            );
        return fromOrigin.Error;
    }

    public async Task<Status> Add(IParser parser, CancellationToken ct = default)
    {
        Status adding = await origin.Add(parser, ct);
        if (adding.IsSuccess)
            await cache.InvalidateAsync(new ParserCacheJson(parser));
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
