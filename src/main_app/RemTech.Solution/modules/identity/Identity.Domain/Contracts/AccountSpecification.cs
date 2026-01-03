namespace Identity.Domain.Contracts;

public sealed class AccountSpecification
{
    public Guid? Id { get; private set; } = null;
    public string? Email { get; private set; } = null;
    public string? Login { get; private set; } = null;
    public bool LockRequired { get; private set; } = false;

    public AccountSpecification WithId(Guid id)
    {
        if (Id.HasValue) return this;
        Id = id;
        return this;
    }
    
    public AccountSpecification WithEmail(string email)
    {
        if (!string.IsNullOrEmpty(Email)) return this;
        Email = email;
        return this;
    }

    public AccountSpecification WithLogin(string login)
    {
        if (!string.IsNullOrEmpty(Login)) return this;
        Login = login;
        return this;
    }

    public AccountSpecification WithLock()
    {
        LockRequired = true;
        return this;
    }
}