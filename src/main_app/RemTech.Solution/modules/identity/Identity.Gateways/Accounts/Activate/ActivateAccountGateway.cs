using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Gateways.Accounts.Responses;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.Activate;

public sealed class ActivateAccountGateway(
    IAccountsStorage persister,
    Serilog.ILogger logger
) 
    : IGateway<ActivateAccountRequest, AccountResponse>
{
    public async Task<Result<AccountResponse>> Execute(ActivateAccountRequest request)
    {
        Result<IAccount> fetching = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: request.Id),
            request.Ct
        );

        if (fetching.IsFailure) return fetching.Error;
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(fetching.Value)));
        Result<IAccount> activated = await account.Activate(persister, request.Ct);
        return activated.IsFailure ? activated.Error : AccountResponse.Represent(activated.Value);
    }
}