using Identity.Core.Accounts.Events;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace Identity.Core.Accounts;

public class Account
{
    protected internal readonly Guid Id;
    protected internal readonly AccountIdentity Identity;
    protected internal readonly string Password;
    protected internal readonly bool Activated;
    protected internal readonly EventsStore? Events;

    public Account Copy(
        Guid? id = null,
        AccountIdentity? identity = null,
        string? password = null,
        bool? activated = null,
        EventsStore? events = null
        )
    {
        Guid nextId = id ?? Id;
        AccountIdentity nextIdentity = identity ?? Identity;
        string nextPassword = password ?? Password;
        bool nextActivated = activated ?? Activated;
        EventsStore? nextStore = events;
        return new Account(nextId, nextIdentity, nextPassword, nextActivated, nextStore);
    }
    
    public virtual void Register()
    {
        if (Activated) throw ErrorException.Conflict("Учетная запись уже зарегистрирована.");
        Events?.Raise(new AccountRegisteredEvent(
            Id: Id, 
            Name: Identity.Name, 
            Email: Identity.Email, 
            Password: Password, 
            Activated: Activated
            ));
    }
    
    public Account WithEventsStore(EventsStore store) => 
        new(Id, Identity, Password, Activated, store);

    public virtual Account ChangePassword(string password)
    {
        Account withOtherPassword = new Account(Id, Identity, password, Activated, Events);
        withOtherPassword.Events?.Raise(new AccountPasswordChangedEvent(
            Id: withOtherPassword.Id, 
            NewPassword: withOtherPassword.Password));
        return withOtherPassword;
    }

    public virtual Account ChangeEmail(string newEmail)
    {
        AccountIdentity @new = Identity.ChangeEmail(newEmail);
        Account updated = new Account(Id, @new, Password, Activated, Events);
        updated.Events?.Raise(new AccountEmailChangedEvent(
            Id: Id,
            OldEmail: Identity.Email,
            NewEmail: updated.Identity.Email
            ));
        return updated;
    }

    public bool VerifyPassword(string inputPassword, CancellationToken ct = default)
    {
        return Password == inputPassword;
    }
    
    public Account(
        Guid id, 
        AccountIdentity identity,
        string password, 
        bool activated,
        EventsStore? events = null)
    {
        Id = id;
        Identity = identity;
        Password = password;
        Activated = activated;
        Events = events;
    }

    protected internal Account(Account origin) :
        this(origin.Id, origin.Identity, origin.Password, origin.Activated, origin.Events)
    { }
}