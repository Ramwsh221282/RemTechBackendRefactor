using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Gateways.Accounts.Responses;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.ChangePassword;

public sealed class ChangeAccountPasswordGateway(
    IAccountsStorage persister,
    Serilog.ILogger logger,
    IAccountCryptography encrypter
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
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(fetching.Value)));
        
        Result<IAccount> passwordChanged = await account.ChangePassword(
            request.NewPassword, 
            persister, 
            encrypter, 
            request.Ct);

        return passwordChanged.IsFailure ? passwordChanged.Error : AccountResponse.Represent(passwordChanged.Value);
    }
}