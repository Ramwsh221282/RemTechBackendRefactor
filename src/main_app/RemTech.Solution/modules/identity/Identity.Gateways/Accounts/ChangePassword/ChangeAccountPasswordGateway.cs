using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Gateways.Accounts.Responses;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.ChangePassword;

public sealed class ChangeAccountPasswordGateway(
    IAccountPersister persister,
    Serilog.ILogger logger,
    IAccountEncrypter encrypter
    )
    : IGateway<ChangeAccountPasswordRequest, AccountResponse>
{
    public async Task<Result<AccountResponse>> Execute(ChangeAccountPasswordRequest request)
    {
        Result<IAccount> fetching = await PersistingAccount.Get(
            persister, 
            new AccountQueryArgs(Id: request.Id, WithLock: true),
            request.Ct);

        if (fetching.IsFailure) return fetching.Error;
        AccountData data = AccountData.Copy(fetching.Value);
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(new Account(data))));
        
        Result<IAccount> passwordChanged = await account.ChangePassword(
            request.NewPassword, 
            persister, 
            encrypter, 
            request.Ct);

        return passwordChanged.IsFailure ? passwordChanged.Error : AccountResponse.Represent(passwordChanged.Value);
    }
}