using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed class GetUserCachedQueryHandler(
    IQueryHandler<GetUserQuery, UserAccountResponse?> inner, 
    HybridCache cache) : IQueryHandler<GetUserQuery, UserAccountResponse?>
{
    private IQueryHandler<GetUserQuery, UserAccountResponse?> Inner { get; } = inner;
    private HybridCache Cache { get; } = cache;
    
    public async Task<UserAccountResponse?> Handle(GetUserQuery query, CancellationToken ct = default)
    {
        string key = $"get_user_{query.AccountId}";
        UserAccountResponse? response = await Cache.GetOrCreateAsync(
            key,
            async cancellationToken => await Inner.Handle(query, cancellationToken),
            cancellationToken: ct);
        return response;
    }
}