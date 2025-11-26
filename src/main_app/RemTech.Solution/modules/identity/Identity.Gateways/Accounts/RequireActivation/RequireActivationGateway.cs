using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequireActivation;

public sealed class RequireActivationGateway(
    IOnAccountActivationRequiredListener listener,
    IAccountsStorage persister,
    Serilog.ILogger logger
    )
    : IGateway<RequireActivationRequest, RequireActivationResponse>
{
    public async Task<Result<RequireActivationResponse>> Execute(RequireActivationRequest request)
    {
        Result<IAccount> fetching = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: request.Id),
            request.Ct
        );

        if (fetching.IsFailure) return fetching.Error;
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(fetching.Value)));
        Result<Unit> reacting = await account.RequireAccountActivation(listener, request.Ct);
        if (reacting.IsFailure) return reacting.Error;
        return new RequireActivationResponse("На почту, указанную при регистрации было отправлено сообщение.");
    }
}