using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Accounts.Decorators;

public abstract class AccountEnvelope(IAccount origin) : IAccount
{
    public virtual Task<Result<IAccount>> Register(
        IAccountCryptography cryptography, 
        IAccountsStorage storage, 
        CancellationToken ct = default) => 
        origin.Register(cryptography, storage, ct);

    public virtual Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountsStorage storage, 
        CancellationToken ct = default) => 
        origin.ChangeEmail(newEmail, storage, ct);

    public virtual Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountsStorage storage, 
        IAccountCryptography cryptography,
        CancellationToken ct = default) =>
        origin.ChangePassword(newPassword, storage, cryptography, ct);

    public virtual Task<Result<Unit>> RequireAccountActivation(
        IOnAccountActivationRequiredListener listener, 
        CancellationToken ct = default) =>
        origin.RequireAccountActivation(listener, ct);

    public virtual Task<Result<Unit>> RequirePasswordReset(
        IOnAccountPasswordResetRequiredListener listener, 
        CancellationToken ct = default) =>
        origin.RequirePasswordReset(listener, ct);

    public virtual Task<Result<IAccount>> Activate(
        IAccountsStorage storage, 
        CancellationToken ct) =>
        origin.Activate(storage, ct);

    public virtual AccountData Represent() =>
        origin.Represent();

    public virtual bool IsActivated() =>
        origin.IsActivated();
}