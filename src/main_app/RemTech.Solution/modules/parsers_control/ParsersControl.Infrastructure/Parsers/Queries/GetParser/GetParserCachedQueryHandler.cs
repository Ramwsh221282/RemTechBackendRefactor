using Microsoft.Extensions.Caching.Hybrid;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParser;

public sealed class GetParserCachedQueryHandler(
    IQueryHandler<GetParserQuery, ParserResponse?> inner,
    HybridCache cache)
    : IQueryHandler<GetParserQuery, ParserResponse?>
{
    private IQueryHandler<GetParserQuery, ParserResponse?> Inner { get; } = inner;
    private HybridCache Cache { get; } = cache;

    public async Task<ParserResponse?> Handle(GetParserQuery query, CancellationToken ct = default)
    {
        string cacheKey = $"get_parser_{query.Id}";
        return await Cache.GetOrCreateAsync(
            key: cacheKey,
            factory: async cancellationToken => await Inner.Handle(query, cancellationToken),
            cancellationToken: ct);
    }
}