using Identity.Application.Accounts;
using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Accounts;

public sealed class PersistingAccount(IAccount account) : AccountEnvelope(account)
{
    private readonly IAccount _account = account;

    public static async Task<Result<IAccount>> Get(
        IAccountPersister persister, 
        AccountQueryArgs args, 
        CancellationToken ct = default)
    {
        IAccount? account = await persister.Get(args, ct);
        if (account == null) return Error.NotFound("Учетная запись не найдена.");
        return Result.Success(account);
    }
    
    public override async Task<Result<Unit>> Register(
        IAccountEncrypter encrypter, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        Result<Unit> result = await _account.Register(encrypter, persister, ct);
        IAccountRepresentation representation = _account.Represent(AccountRepresentation.Empty());
        Result<Unit> hasUniqueEmail = await ValidateEmailUniquesness(representation, persister, ct);
        if (hasUniqueEmail.IsFailure) return hasUniqueEmail.Error;
        Result<Unit> hasUniqueName = await ValidateNameUniquesness(representation, persister, ct);
        if (hasUniqueName.IsFailure) return hasUniqueName.Error;
        await persister.Persist(_account, ct);
        return result;
    }

    public override async Task<Result<IAccount>> Activate(
        IAccountPersister persister, 
        CancellationToken ct)
    {
        Result<IAccount> activation = await _account.Activate(persister, ct);
        if (activation.IsFailure) return activation.Error;
        await persister.Update(activation.Value, ct);
        return activation;
    }

    public override async Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountPersister persister, 
        CancellationToken ct = default)
    {
        Result<IAccount> updating = await  _account.ChangeEmail(newEmail, persister, ct);
        if (updating.IsFailure) return updating.Error;
        IAccountRepresentation representation = updating.Value.Represent(AccountRepresentation.Empty());
        Result<Unit> hasUniqueEmail = await ValidateEmailUniquesness(representation, persister, ct);
        if (hasUniqueEmail.IsFailure) return hasUniqueEmail.Error;
        await persister.Persist(updating.Value, ct);
        return updating;
    }

    public override async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountPersister persister, 
        IAccountEncrypter encrypter,
        CancellationToken ct = default)
    {
        Result<IAccount> updating = await _account.ChangePassword(newPassword, persister, encrypter, ct);
        if (updating.IsFailure) return updating.Error;
        await persister.Persist(updating.Value, ct);
        return updating;
    }
    
    public async Task<Result<Unit>> ValidateNameUniquesness(
        IAccountRepresentation representation,
        IAccountPersister persister,
        CancellationToken ct = default
    )
    {
        return await ValidateNameUniquesness(representation.Data, persister, ct);
    }
    
    public async Task<Result<Unit>> ValidateNameUniquesness(
        IAccountData data,
        IAccountPersister persister,
        CancellationToken ct = default
    )
    {
        return await ValidateNameUniquesness(data.Name, persister, ct);
    }
    
    public async Task<Result<Unit>> ValidateNameUniquesness(
        string name,
        IAccountPersister persister,
        CancellationToken ct = default)
    {
        AccountQueryArgs args = new(Name: name);
        IAccount? withName = await persister.Get(args, ct);
        if (withName != null) return Error.Conflict($"Учетная запись с названием: {name} уже существует.");
        return Unit.Value;
    }
    
    public async Task<Result<Unit>> ValidateEmailUniquesness(
        string email,
        IAccountPersister persister,  
        CancellationToken ct = default)
    {
        AccountQueryArgs args = new(Email: email);
        IAccount? withEmail = await persister.Get(args, ct);
        if (withEmail != null) return Error.Conflict($"Учетная запись с почтой: {email} уже существует.");
        return Unit.Value;
    }

    public async Task<Result<Unit>> ValidateEmailUniquesness(
        IAccountRepresentation representation,
        IAccountPersister persister,
        CancellationToken ct = default
    )
    {
        return await ValidateEmailUniquesness(representation.Data, persister, ct);
    }
    
    public async Task<Result<Unit>> ValidateEmailUniquesness(
        IAccountData data,
        IAccountPersister persister,
        CancellationToken ct = default)
    {
        return await ValidateEmailUniquesness(data.Email, persister, ct);
    }
}