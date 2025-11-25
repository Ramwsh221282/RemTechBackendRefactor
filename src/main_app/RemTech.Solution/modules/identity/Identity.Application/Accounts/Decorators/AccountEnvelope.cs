using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Accounts.Decorators;

public abstract class AccountEnvelope(IAccount origin) : IAccount
{
    public virtual Task<Result<Unit>> Register(
        IAccountEncrypter encrypter, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        return origin.Register(encrypter, persister, ct);
    }

    public virtual Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        return origin.ChangeEmail(newEmail, persister, ct);
    }

    public virtual Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountPersister persister, 
        IAccountEncrypter encrypter,
        CancellationToken ct = default)
    {
        return origin.ChangePassword(newPassword, persister, encrypter, ct);
    }

    public virtual Task<Result<Unit>> RequireAccountActivation(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default)
    {
        return origin.RequireAccountActivation(publisher, ct);
    }

    public virtual Task<Result<Unit>> RequirePasswordReset(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default)
    {
        return origin.RequirePasswordReset(publisher, ct);
    }

    public virtual Task<Result<IAccount>> Activate(
        IAccountPersister persister, 
        CancellationToken ct)
    {
        return origin.Activate(persister, ct);
    }

    public virtual IAccountRepresentation Represent(IAccountRepresentation representation)
    {
        return origin.Represent(representation);
    }

    public virtual bool IsActivated()
    {
        return origin.IsActivated();
    }
}