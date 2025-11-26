using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequirePasswordReset;

public sealed class RequirePasswordResetGateway(
    IAccountsStorage persister,
    IOnAccountPasswordResetRequiredListener listener,
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
        IAccount account = new LoggingAccount(logger, new ValidAccount(fetching.Value));
        Result<Unit> requiring = await account.RequirePasswordReset(listener, request.Ct);
        if (requiring.IsFailure) return requiring.Error;
        return new RequirePasswordResetResponse(
            "На активированную почту учетной записи было отправлено сообщение.");
    }
}