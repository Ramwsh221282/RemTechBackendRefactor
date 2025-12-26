using Identity.Contracts.Accounts;
using Identity.Infrastructure.Accounts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class GetAccount(IServiceProvider sp)
{
    public async Task<Result<AccountData>> Invoke(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IAccountsStorage persister = scope.Resolve<IAccountsStorage>();
        Result<IAccount> account = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: id),
            CancellationToken.None
        );
        
        if (account.IsFailure) return account.Error;
        AccountData representation = account.Value.Represent();
        return Result.Success(representation);
    }
}