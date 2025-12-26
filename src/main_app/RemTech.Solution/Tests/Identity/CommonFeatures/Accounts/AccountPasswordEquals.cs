using Identity.Contracts.Accounts;
using Identity.Infrastructure.Accounts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class AccountPasswordEquals(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid accountId, string inputPassword)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IAccountsStorage persister = scope.Resolve<IAccountsStorage>();
        IAccountCryptography decrypted = scope.Resolve<IAccountCryptography>();
        Result<IAccount> account = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: accountId),
            ct
        );
        if (account.IsFailure) throw new InvalidOperationException($"Account with ID: {accountId} does not exist");
        IAccount withDecryptedData = await decrypted.Decrypt(account.Value, ct);
        string decryptedPassword = withDecryptedData.Represent().Password;
        return decryptedPassword == inputPassword;
    }
}