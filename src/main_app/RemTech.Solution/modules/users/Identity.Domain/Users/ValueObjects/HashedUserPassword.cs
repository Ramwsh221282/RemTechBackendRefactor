using Identity.Domain.Users.Ports.Passwords;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.ValueObjects;

public sealed record HashedUserPassword
{
    public string Password { get; }

    private HashedUserPassword(string password) => Password = password;

    public HashedUserPassword(IPasswordManager manager)
    {
        string guidString = Guid.NewGuid().ToString();
        Password = manager.Hash(guidString);
    }

    public HashedUserPassword(UserPassword password, IPasswordManager manager) =>
        Password = manager.Hash(password.Password);

    public bool VerifyPassword(UserPassword password, IPasswordManager manager, out Error error)
    {
        bool verified = manager.Verify(password.Password, Password);
        if (!verified)
            error = new Error("Пароль неверный.", ErrorCodes.Unauthorized);

        error = Error.None();
        return verified;
    }

    public static HashedUserPassword Random(IPasswordManager manager)
    {
        string password = manager.Hash(Guid.NewGuid().ToString());
        return new HashedUserPassword(password);
    }
}
