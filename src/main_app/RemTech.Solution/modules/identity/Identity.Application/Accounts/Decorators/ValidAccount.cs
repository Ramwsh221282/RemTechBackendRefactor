using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Accounts.Decorators;

public sealed class ValidAccount(IAccount account) : AccountEnvelope(account)
{
    private readonly AccountValidationTools _tools = new();
    private readonly IAccount _account = account;

    public override async Task<Result<IAccount>> Register(
        IAccountCryptography encrypter, 
        IAccountsStorage persister, 
        CancellationToken ct = default)
    {
        Result<Unit> validation = _tools.ValidateData(_account.Represent());
        if (validation.IsFailure) return validation.Error;
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        return await _account.Register(encrypter, persister, ct);
    }

    public override async Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountsStorage persister, 
        CancellationToken ct = default)
    {
        if (!IsActivated()) return Error.Conflict("Учетная запись не активирована.");
        Result<Unit> validation = _tools.ValidateProperty(_account.Represent(), email: newEmail);
        if (validation.IsFailure) return validation.Error;
        return await _account.ChangeEmail(newEmail, persister, ct);
    }

    public override async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountsStorage persister, 
        IAccountCryptography encrypter,
        CancellationToken ct = default)
    {
        if (!IsActivated()) return Error.Conflict("Учетная запись не активирована.");
        Result<Unit> validation = _tools.ValidateProperty(_account.Represent(), password: newPassword);
        if (validation.IsFailure) return validation.Error;
        return await _account.ChangePassword(newPassword, persister, encrypter, ct);
    }

    public override async Task<Result<Unit>> RequireAccountActivation(
        IOnAccountActivationRequiredListener listener, 
        CancellationToken ct = default)
    {
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        return await _account.RequireAccountActivation(listener, ct);
    }

    public override async Task<Result<Unit>> RequirePasswordReset(
        IOnAccountPasswordResetRequiredListener publisher, 
        CancellationToken ct = default)
    {
        if (!IsActivated()) return Error.Conflict("Учетная запись не активирована.");
        return await _account.RequirePasswordReset(publisher, ct);
    }

    public override async Task<Result<IAccount>> Activate(IAccountsStorage persister, CancellationToken ct)
    {
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        return await _account.Activate(persister, ct);
    }
}