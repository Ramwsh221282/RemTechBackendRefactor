using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequireActivation;

public sealed class RequireActivationGateway(
    IAccountMessagePublisher publisher,
    IAccountPersister persister,
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

        AccountData data = AccountData.Copy(fetching.Value);
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(new Account(data))));
        Result<Unit> requiring = await account.RequireAccountActivation(publisher, request.Ct);
        return requiring.IsFailure ? requiring.Error : 
            new RequireActivationResponse("На почту, указанную при регистрации было отправлено сообщение.");
    }
}