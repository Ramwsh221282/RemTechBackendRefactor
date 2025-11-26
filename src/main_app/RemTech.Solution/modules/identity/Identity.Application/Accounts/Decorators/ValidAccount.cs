using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Accounts.Decorators;

public sealed class ValidAccount(IAccount account) : AccountEnvelope(account)
{
    private readonly AccountValidationTools _tools = new();
    private readonly IAccount _account = account;

    public override async Task<Result<IAccount>> Register(
        IAccountEncrypter encrypter, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        IAccountRepresentation representation = Represent(AccountRepresentation.Empty());
        Result<Unit> validation = _tools.ValidateData(representation.Data);
        if (validation.IsFailure) return validation.Error;
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        return await _account.Register(encrypter, persister, ct);
    }

    public override async Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        if (!IsActivated()) return Error.Conflict("Учетная запись не активирована.");
        Result<Unit> validation = _tools.ValidateProperty(Represent(AccountRepresentation.Empty()), email: newEmail);
        if (validation.IsFailure) return validation.Error;
        return await _account.ChangeEmail(newEmail, persister, ct);
    }

    public override async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountPersister persister, 
        IAccountEncrypter encrypter,
        CancellationToken ct = default)
    {
        if (!IsActivated()) return Error.Conflict("Учетная запись не активирована.");
        Result<Unit> validation = _tools.ValidateProperty(Represent(AccountRepresentation.Empty()), password: newPassword);
        if (validation.IsFailure) return validation.Error;
        return await _account.ChangePassword(newPassword, persister, encrypter, ct);
    }

    public override async Task<Result<Unit>> RequireAccountActivation(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default)
    {
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        return await _account.RequireAccountActivation(publisher, ct);
    }

    public override async Task<Result<Unit>> RequirePasswordReset(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default)
    {
        if (!IsActivated()) return Error.Conflict("Учетная запись не активирована.");
        return await _account.RequirePasswordReset(publisher, ct);
    }

    public override async Task<Result<IAccount>> Activate(IAccountPersister persister, CancellationToken ct)
    {
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        return await _account.Activate(persister, ct);
    }
}