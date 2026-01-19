using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Vehicles.Infrastructure.Models.Queries.GetModel;

public sealed class GetModelCachingQueryHandler(
    HybridCache cache,
    IQueryHandler<GetModelQuery, ModelResponse?> handler
) : IQueryExecutorWithCache<GetModelQuery, ModelResponse?>
{
    private HybridCache Cache { get; } = cache;
    private IQueryHandler<GetModelQuery, ModelResponse?> Handler { get; } = handler;

    public async Task<ModelResponse?> ExecuteWithCache(
        GetModelQuery query,
        CancellationToken ct = default
    ) => await GetModelResponse(CreateKey(query), query, ct);

    private async Task<ModelResponse?> GetModelResponse(
        string key,
        GetModelQuery query,
        CancellationToken ct
    ) =>
        await Cache.GetOrCreateAsync(
            key,
            async cancellationToken => await Handler.Handle(query, cancellationToken),
            cancellationToken: ct
        );

    private static string CreateKey(GetModelQuery query) => $"{nameof(GetModelQuery)}_{query}";
}
