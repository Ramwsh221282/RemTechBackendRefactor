using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Persistence;
using Microsoft.Extensions.Caching.Hybrid;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

public sealed class GetUserQueryHandler(IAccountsRepository accounts, HybridCache cache) : IQueryHandler<GetUserQuery, UserAccountResponse?>
{
    private IAccountsRepository Accounts { get; } = accounts;
    
    public async Task<UserAccountResponse?> Handle(GetUserQuery query, CancellationToken ct = default)
    {
        AccountSpecification spec = new AccountSpecification().WithId(query.AccountId);
        Result<Account> account = await Accounts.Get(spec, ct: ct);
        if (account.IsFailure) return null;
        return UserAccountResponse.Create(account.Value);
    }
}