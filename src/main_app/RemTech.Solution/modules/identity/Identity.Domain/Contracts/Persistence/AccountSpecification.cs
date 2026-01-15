namespace Identity.Domain.Contracts.Persistence;

public sealed class AccountSpecification
{
    public Guid? Id { get; private set; } = null;
    public string? Email { get; private set; } = null;
    public string? Login { get; private set; } = null;
    public bool LockRequired { get; private set; } = false;
    public string? RefreshToken { get; private set; } = null;

    public AccountSpecification WithRefreshToken(string refreshToken)
    {
        if (!string.IsNullOrEmpty(RefreshToken))
            return this;
        RefreshToken = refreshToken;
        return this;
    }

    public AccountSpecification WithId(Guid id)
    {
        if (Id.HasValue)
            return this;
        Id = id;
        return this;
    }

    public AccountSpecification WithEmail(string email)
    {
        if (!string.IsNullOrEmpty(Email))
            return this;
        Email = email;
        return this;
    }

    public AccountSpecification WithLogin(string login)
    {
        if (!string.IsNullOrEmpty(Login))
            return this;
        Login = login;
        return this;
    }

    public AccountSpecification WithLock()
    {
        LockRequired = true;
        return this;
    }
}
