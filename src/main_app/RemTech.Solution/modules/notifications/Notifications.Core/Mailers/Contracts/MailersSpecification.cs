namespace Notifications.Core.Mailers.Contracts;

public sealed class MailersSpecification
{
    public Guid? Id { get; private set; }
    public string? Email { get; private set; }
    public bool? LockRequired { get; private set; }

    public MailersSpecification WithId(Guid id)
    {
        if (Id.HasValue) return this;
        Id = id;
        return this;
    }
    
    public MailersSpecification WithEmail(string email)
    {
        if (!string.IsNullOrWhiteSpace(Email)) return this;
        Email = email;
        return this;
    }
    
    public MailersSpecification WithLockRequired()
    {
        LockRequired = true;
        return this;
    }
}