using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Gateways.Accounts.Responses;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.ChangeEmail;

public sealed class ChangeAccountEmailGateway(
    IAccountPersister persister,
    Serilog.ILogger logger
    ) :
    IGateway<ChangeAccountEmailRequest, AccountResponse>
{
    public async Task<Result<AccountResponse>> Execute(ChangeAccountEmailRequest request)
    {
        Result<IAccount> recieving = await PersistingAccount.Get(
            persister, 
            new AccountQueryArgs(Id: request.Id, WithLock: true),
            request.Ct);
        
        if (recieving.IsFailure) return recieving.Error;
        IAccountRepresentation representation = recieving.Value.Represent(AccountRepresentation.Empty());
        AccountData data = AccountData.Copy(representation.Data);
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(new Account(data))));
        Result<IAccount> emailChanged = await account.ChangeEmail(request.Email, persister, request.Ct);
        return emailChanged.IsFailure ? emailChanged.Error : AccountResponse.Represent(emailChanged.Value);
    }
}