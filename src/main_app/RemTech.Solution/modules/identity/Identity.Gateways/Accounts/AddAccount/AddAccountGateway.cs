using Identity.Core;
using Identity.Core.Accounts;
using Identity.Core.Accounts.Decorators;
using Identity.Infrastructure.Cryptography;
using Identity.Infrastructure.Logging;
using Identity.Infrastructure.NpgSql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.AddAccount;

public sealed class AddAccountGateway(
    AccountsLogger logger,
    EventsStore events,
    NpgSqlAccountsStorage storage,
    AesCryptography cryptography,
    AddAccountResultWaiter waiter) :
    IGateway<AddAccountRequest, AddAccountResponse>
{
    private readonly AccountsLogger _logger = logger;
    private readonly EventsStore _events = events;
    private readonly NpgSqlAccountsStorage _storage = storage;
    private readonly AesCryptography _cryptography = cryptography;
    private readonly AddAccountResultWaiter _waiter = waiter;
    
    public async Task<Result<AddAccountResponse>> Execute(AddAccountRequest request)
    {
        return await new AsyncOperation<AddAccountResponse>(async () =>
        {
            AccountIdentity identity = new(request.Email, request.Name);
            Account account = new(Guid.NewGuid(), identity, request.Password, false, _events);
            Account valid = new ValidAccount(account).Valid();
            Account encrypted = await new EncryptedAccount(_cryptography, valid).Account(request.Ct);
            encrypted.Register();
            await _storage.ProcessDatabaseCommands(request.Ct);
            _logger.ProcessLogging();
            return _waiter.ReadResult();
        }).Process();
    }
}