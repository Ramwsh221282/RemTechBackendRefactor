using Identity.Domain.Accounts.Models.Events;
using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.PasswordRequirements;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Models;

public sealed class Account(
    AccountId id, 
    AccountEmail email, 
    AccountPassword password, 
    AccountLogin login, 
    AccountActivationStatus activationStatus,
    AccountPermissionsCollection permissions) : IDomainEventBearer
{
    private Account(Account account) 
        : this(account.Id, account.Email, account.Password, account.Login, account.ActivationStatus, account.Permissions.Clone())
    { }

    private List<IDomainEvent> _events = [];
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
        _events.Add(new AccountActivatedEvent(this));
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
        Result<Unit> finishing = ticket.FinishBy(Id.Value);
        if (finishing.IsFailure) return finishing.Error;
        _events.Add(new AccountClosedTicketEvent(this, ticket));
        return Unit.Value;
    }
    
    public Result<Unit> ChangePassword(
        AccountPassword password, 
        IPasswordHasher hasher,
        IAccountPasswordRequirement requirement,
        CancellationToken ct = default)
    {
        Result<Unit> validation = requirement.Satisfies(password);
        if (validation.IsFailure) return validation.Error;
        Password = Password.HashBy(hasher, ct);
        return Unit.Value;
    }

    public Result<Unit> VerifyPassword(string input, IPasswordHasher hasher)
    {
        bool verified = Password.Verify(input, hasher);
        return verified ? Result.Success(Unit.Value) : Error.Validation("Неверный пароль.");
    }

    public void ChangeEmail(AccountEmail email) => Email = email;
    
    public static Account Create(
        AccountEmail email, 
        AccountLogin login, 
        AccountPassword password,
        AccountActivationStatus status)
    {
        Account account = New(email, login, password);
        account.ActivationStatus = status;
        return account;
    }
    
    public static Account New(AccountEmail email, AccountLogin login, AccountPassword password)
    {
        AccountId id = AccountId.New();
        AccountActivationStatus activationStatus = AccountActivationStatus.NotActivated();
        AccountPermissionsCollection permissions = AccountPermissionsCollection.Empty(id);
        Account account = new Account(id, email, password, login, activationStatus, permissions);
        account._events.Add(new NewAccountCreatedEvent(account));
        return account;
    }

    public Account Copy()
    {
        Account account = new(this);   
        account._events = [.._events];
        return account;
    }

    public IReadOnlyList<IDomainEvent> Events => _events;
}