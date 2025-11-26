using Identity.Application.Accounts.Messages;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Accounts;

public sealed class Account(AccountData data) : IAccount
{
    public async Task<Result<IAccount>> Register(
        IAccountCryptography encrypter, 
        IAccountsStorage persister, 
        CancellationToken ct = default)
    {
        IAccount encrypted = await encrypter.Encrypt(this, ct);
        return Result.Success(encrypted);
    }

    public Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountsStorage persister, 
        CancellationToken ct = default)
    {
        AccountData @new = data with { Email = newEmail };
        Account updated = new(@new);
        return Task.FromResult(Result.Success<IAccount>(updated));
    }

    public async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountsStorage persister, 
        IAccountCryptography encrypter,
        CancellationToken ct = default)
    {
        AccountData @new = data with { Password = newPassword };
        Account updated = new(@new);
        IAccount encrypted = await encrypter.Encrypt(updated, ct);
        return Result.Success(encrypted);
    }

    public async Task<Result<Unit>> RequireAccountActivation(
        IOnAccountActivationRequiredListener publisher, 
        CancellationToken ct = default)
    {
        if (IsActivated()) return Error.Conflict("Учетная запись уже активирована.");
        AccountActivationTicketRequired message = new(data.Id, data.Email);
        return await publisher.React(data, ct);
    }

    public async Task<Result<Unit>> RequirePasswordReset(
        IOnAccountPasswordResetRequiredListener publisher, 
        CancellationToken ct = default)
    {
        AccountPasswordResetRequired message = new(data.Id, data.Email);
        Result<Unit> result = await publisher.React(data, ct);
        if (result.IsFailure) return result.Error;
        return result.Value;
    }

    public Task<Result<IAccount>> Activate(
        IAccountsStorage storage, 
        CancellationToken ct)
    {
        AccountData @new = data with { Activated = true };
        Account updated = new(@new);
        return Task.FromResult<Result<IAccount>>(updated);
    }

    public AccountData Represent()
    {
        return data;
    }

    public bool IsActivated()
    {
        return data.Activated;
    }
}