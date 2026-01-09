using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

public sealed class GetParsersCachedQueryHandler(
    IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>> inner,
    HybridCache cache) 
    : IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>>
{
    public async Task<IEnumerable<ParserResponse>> Handle(GetParsersQuery query, CancellationToken ct = default)
    {
        string cacheKey = ParserCacheContants.Array;
        return await cache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken =>
            {
                IEnumerable<ParserResponse> response = await inner.Handle(query, cancellationToken);
                return response;
            },
            cancellationToken: ct
        );
    }
}