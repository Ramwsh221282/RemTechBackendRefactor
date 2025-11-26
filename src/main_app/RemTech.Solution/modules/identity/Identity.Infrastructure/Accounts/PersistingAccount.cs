using Identity.Application.Accounts.Decorators;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Accounts;

public sealed class PersistingAccount(IAccount account) : AccountEnvelope(account)
{
    private readonly IAccount _account = account;

    public static async Task<Result<IAccount>> Get(
        IAccountsStorage storage, 
        AccountQueryArgs args, 
        CancellationToken ct = default)
    {
        IAccount? account = await storage.Fetch(args, ct);
        return account == null ? Error.NotFound("Учетная запись не найдена.") : Result.Success(account);
    }
    
    public override async Task<Result<IAccount>> Register(
        IAccountCryptography cryptography, 
        IAccountsStorage storage, 
        CancellationToken ct = default)
    {
        Result<IAccount> result = await _account.Register(cryptography, storage, ct);
        Result<Unit> hasUniqueEmail = await ValidateEmailUniquesness(result.Value, storage, ct);
        if (hasUniqueEmail.IsFailure) return hasUniqueEmail.Error;
        Result<Unit> hasUniqueName = await ValidateNameUniquesness(result.Value, storage, ct);
        if (hasUniqueName.IsFailure) return hasUniqueName.Error;
        await storage.Persist(result.Value, ct);
        return result;
    }

    public override async Task<Result<IAccount>> Activate(
        IAccountsStorage storage, 
        CancellationToken ct)
    {
        Result<IAccount> activation = await _account.Activate(storage, ct);
        if (activation.IsFailure) return activation.Error;
        await storage.Update(activation.Value, ct);
        return activation;
    }

    public override async Task<Result<IAccount>> ChangeEmail(
        string newEmail, 
        IAccountsStorage storage, 
        CancellationToken ct = default)
    {
        Result<IAccount> updating = await  _account.ChangeEmail(newEmail, storage, ct);
        if (updating.IsFailure) return updating.Error;
        Result<Unit> hasUniqueEmail = await ValidateEmailUniquesness(updating.Value, storage, ct);
        if (hasUniqueEmail.IsFailure) return hasUniqueEmail.Error;
        await storage.Update(updating.Value, ct);
        return updating;
    }

    public override async Task<Result<IAccount>> ChangePassword(
        string newPassword, 
        IAccountsStorage storage, 
        IAccountCryptography cryptography,
        CancellationToken ct = default)
    {
        Result<IAccount> updating = await _account.ChangePassword(newPassword, storage, cryptography, ct);
        if (updating.IsFailure) return updating.Error;
        await storage.Update(updating.Value, ct);
        return updating;
    }

    private static async Task<Result<Unit>> ValidateNameUniquesness(
        IAccount account,
        IAccountsStorage storage,
        CancellationToken ct = default)
    {
        AccountData data = account.Represent();
        AccountQueryArgs args = new(Name: data.Name);
        IAccount? withName = await storage.Fetch(args, ct);
        if (withName != null) return Error.Conflict($"Учетная запись с названием: {data.Name} уже существует.");
        return Unit.Value;
    }

    private static async Task<Result<Unit>> ValidateEmailUniquesness(
        IAccount account,
        IAccountsStorage storage,  
        CancellationToken ct = default)
    {
        AccountData data = account.Represent();
        AccountQueryArgs args = new(Email: data.Email);
        IAccount? withEmail = await storage.Fetch(args, ct);
        if (withEmail != null) return Error.Conflict($"Учетная запись с почтой: {data.Email} уже существует.");
        return Unit.Value;
    }
}