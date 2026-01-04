using Identity.Domain.Contracts;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public sealed class Account(
    AccountId id, 
    AccountEmail email, 
    AccountPassword password, 
    AccountLogin login, 
    AccountActivationStatus activationStatus,
    AccountPermissionsCollection permissions)
{
    private Account(Account account) 
        : this(account.Id, account.Email, account.Password, account.Login, account.ActivationStatus, account.Permissions.Clone())
    { }

    public AccountId Id { get; private set; } = id;
    public AccountEmail Email { get; private set; } = email;
    public AccountPassword Password { get; private set; } = password;
    public AccountLogin Login { get; private set; } = login;
    public AccountActivationStatus ActivationStatus { get; private set; } = activationStatus;
    public AccountPermissionsCollection Permissions { get; private set; } = permissions;
    public int PermissionsCount => Permissions.Count;
    public IReadOnlyList<Permission> PermissionsList => Permissions.Permissions;

    public Result<Unit> Activate()
    {
        if (ActivationStatus.IsActivated())
            return Error.Conflict("Учетная запись уже активирована.");
        ActivationStatus = AccountActivationStatus.Activated();
        return Result.Success(Unit.Value);
    }

    public Result<AccountTicket> CreateTicket(string purpose)
    {
        return AccountTicket.New(this, purpose);
    }

    public Result<Unit> AddPermissions(IEnumerable<Permission> permissions)
    {
        List<string> errors = [];
        foreach (Permission permission in permissions)
        {
            Result<Unit> add = Permissions.Add(permission);
            if (add.IsFailure) errors.Add(add.Error.Message);
        }
        if (errors.Any()) return Error.Conflict(string.Join(", ", errors));
        return Result.Success(Unit.Value);
    }
    
    public Result<Unit> AddPermission(Permission permission)
    {
        Result<Unit> add = Permissions.Add(permission);
        return add.IsFailure ? add.Error : Unit.Value;
    }
    
    public Result<Unit> CloseTicket(AccountTicket ticket)
    {
        return ticket.AccountId != Id ? Error.NotFound("Заявка не найдена.") : ticket.Finish();
    }
    
    public async Task<Result<Unit>> ChangePassword(
        AccountPassword password, 
        IPasswordCryptography encrypter,
        IAccountPasswordRequirement requirement,
        CancellationToken ct = default)
    {
        Result<Unit> validation = requirement.Satisfies(password);
        if (validation.IsFailure) return validation.Error;
        Password = await encrypter.Encrypt(password, ct);
        return Unit.Value;
    }

    public void ChangeEmail(AccountEmail email) => Email = email;

    public static Account New(AccountEmail email, AccountLogin login, AccountPassword password)
    {
        AccountId id = AccountId.New();
        AccountActivationStatus activationStatus = AccountActivationStatus.NotActivated();
        AccountPermissionsCollection permissions = AccountPermissionsCollection.Empty(id);
        return new Account(id, email, password, login, activationStatus, permissions);
    }

    public Account Copy() => new(this);
}