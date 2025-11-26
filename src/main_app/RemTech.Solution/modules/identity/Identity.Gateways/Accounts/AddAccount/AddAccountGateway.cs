using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Decorators;
using Identity.Gateways.Accounts.Responses;
using Identity.Infrastructure.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.AddAccount;

public sealed class AddAccountGateway(
    IAccountEncrypter encrypter,
    IAccountPersister persister,
    Serilog.ILogger logger
    ) :
    IGateway<AddAccountRequest, AccountResponse>
{
    public async Task<Result<AccountResponse>> Execute(AddAccountRequest request)
    {
        AccountData data = AccountData.New(request.Name, request.Email, request.Password);
        IAccount account = new LoggingAccount(logger, new ValidAccount(new PersistingAccount(new Account(data))));
        Result<IAccount> result = await account.Register(encrypter, persister, request.Ct);
        return result.IsFailure ? result.Error : AccountResponse.Represent(result.Value);
    }
}