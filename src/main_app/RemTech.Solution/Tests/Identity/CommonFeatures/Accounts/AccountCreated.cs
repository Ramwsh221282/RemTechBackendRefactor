using Identity.Contracts.Accounts;
using Identity.Infrastructure.Accounts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class AccountCreated(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IAccountsStorage persister = scope.Resolve<IAccountsStorage>();
        Result<IAccount> account = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: id),
            CancellationToken.None
        );
        return account.IsSuccess;
    }
}

