using Identity.Domain.Users.Ports.Passwords;

namespace Identity.Domain.Users.ValueObjects;

public sealed record HashedUserPassword
{
    public string Password { get; }

    public HashedUserPassword(IPasswordManager manager)
    {
        string guidString = Guid.NewGuid().ToString();
        Password = manager.Hash(guidString);
    }

    public HashedUserPassword(UserPassword password, IPasswordManager manager) =>
        Password = manager.Hash(password.Password);
}
