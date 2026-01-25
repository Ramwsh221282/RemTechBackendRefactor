using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.CacheQuery;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed class GetUserCachedQueryHandler(
    IQueryHandler<GetUserQuery, UserAccountResponse?> inner,
    HybridCache cache
) : IQueryExecutorWithCache<GetUserQuery, UserAccountResponse?>
{
    private IQueryHandler<GetUserQuery, UserAccountResponse?> Inner { get; } = inner;
    private HybridCache Cache { get; } = cache;

    public async Task<UserAccountResponse?> ExecuteWithCache(
        GetUserQuery query,
        CancellationToken ct = default
    ) =>
        query switch
        {
            GetUserByIdQuery byId => await HandleAsIdQuery(byId, ct),
            GetUserByRefreshTokenQuery byToken => await HandleAsRefreshTokenQuery(byToken, ct),
            _ => null,
        };

    private async Task<UserAccountResponse?> HandleAsRefreshTokenQuery(
        GetUserByRefreshTokenQuery query,
        CancellationToken ct
    )
    {
        string key = $"get_user_{query.RefreshToken}";
        return await Cache.GetOrCreateAsync(
            key,
            async cancellationToken => await Inner.Handle(query, cancellationToken),
            cancellationToken: ct
        );
    }

    private async Task<UserAccountResponse?> HandleAsIdQuery(
        GetUserByIdQuery query,
        CancellationToken ct
    )
    {
        string key = $"get_user_{query.AccountId}";
        return await Cache.GetOrCreateAsync(
            key,
            async cancellationToken => await Inner.Handle(query, cancellationToken),
            cancellationToken: ct
        );
    }
}
