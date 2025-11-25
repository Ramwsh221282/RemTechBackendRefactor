using Identity.Application.Accounts.Messages;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Accounts;

public sealed class Account(AccountData data) : IAccount
{
    public async Task<Result<Unit>> Register(
        IAccountEncrypter encrypter, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        IAccount encrypted = await encrypter.Encrypt(this, ct);
        return Unit.Value;
    }

    public Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        AccountData @new = data with { Email = newEmail };
        Account updated = new(data);
        return Task.FromResult(Result.Success<IAccount>(updated));
    }

    public async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountPersister persister, 
        IAccountEncrypter encrypter,
        CancellationToken ct = default)
    {
        AccountData @new = data with { Password = newPassword };
        Account updated = new(data);
        IAccount encrypted = await encrypter.Encrypt(this, ct);
        return Result.Success(encrypted);
    }

    public async Task<Result<Unit>> RequireAccountActivation(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default)
    {
        AccountActivationTicketRequired message = new(data.Id, data.Email);
        await publisher.Publish(message, ct);
        return Unit.Value;
    }

    public async Task<Result<Unit>> RequirePasswordReset(
        IAccountMessagePublisher publisher, 
        CancellationToken ct = default)
    {
        AccountPasswordResetRequired message = new(data.Id, data.Email);
        await publisher.Publish(message, ct);
        return Unit.Value;
    }

    public Task<Result<IAccount>> Activate(
        IAccountPersister persister, 
        CancellationToken ct)
    {
        AccountData @new = data with { Activated = true };
        Account updated = new(@new);
        return Task.FromResult<Result<IAccount>>(updated);
    }

    public IAccountRepresentation Represent(
        IAccountRepresentation representation)
    {
        IAccountRepresentation withId = representation.AddId(data.Id);
        IAccountRepresentation withName = withId.AddName(data.Name);
        IAccountRepresentation withEmail = withName.AddEmail(data.Email);
        IAccountRepresentation withPassword = withEmail.AddPassword(data.Password);
        IAccountRepresentation withActivated = withPassword.AddActivated(data.Activated);
        return withActivated;
    }

    public bool IsActivated()
    {
        return data.Activated;
    }
}