using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequirePasswordReset;

public sealed class RequirePasswordResetGateway(
    IAccountPersister persister,
    IAccountMessagePublisher publisher,
    Serilog.ILogger logger
) 
    : IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse>
{
    public async Task<Result<RequirePasswordResetResponse>> Execute(RequirePasswordResetRequest request)
    {
        Result<IAccount> fetching = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: request.Id),
            request.Ct
        );

        if (fetching.IsFailure) return fetching.Error;
        AccountData data = AccountData.Copy(fetching.Value);
        IAccount account = new LoggingAccount(logger, new ValidAccount(new Account(data)));
        Result<Unit> requiring = await account.RequirePasswordReset(publisher, request.Ct);
        return requiring.IsFailure
            ? requiring.Error
            : new RequirePasswordResetResponse("На активированную почту учетной записи было отправлено сообщение.");
    }
}