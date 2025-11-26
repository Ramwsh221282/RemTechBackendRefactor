using Identity.Contracts.Shared.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.Accounts;

public interface IAccountsStorage :
    IEntityPersister<IAccount>,
    IEntityUpdater<IAccount>,
    IEntityFetcher<IAccount, AccountQueryArgs> { }

public interface IAccountCryptography
{
    Task<IAccount> Encrypt(IAccount account, CancellationToken ct = default);
    Task<IAccount> Decrypt(IAccount account, CancellationToken ct = default);
}

public interface IOnAccountActivationRequiredListener
{
    Task<Result<Unit>> React(AccountData data, CancellationToken ct = default);
}

public interface IOnAccountPasswordResetRequiredListener
{
    Task<Result<Unit>> React(AccountData data, CancellationToken ct = default);
}

public sealed record AccountQueryArgs(
    Guid? Id = null, 
    string? Name = null, 
    string? Email = null,
    bool WithLock = false) : EntityFetchArgs;