using Identity.Domain.Users.Ports.Passwords;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Profile.ValueObjects;

public sealed record HashedUserPassword
{
    public string Password { get; }

    private HashedUserPassword(string password) => Password = password;

    private HashedUserPassword(IStringHashAlgorithm manager)
    {
        string guidString = Guid.NewGuid().ToString();
        Password = manager.Hash(guidString);
    }

    public HashedUserPassword(UserPassword password, IStringHashAlgorithm manager) =>
        Password = manager.Hash(password.Password);

    public static Status<HashedUserPassword> Create(string password)
    {
        if (string.IsNullOrEmpty(password))
            return Error.Validation("Пароль пользователя пустой.");
        return new HashedUserPassword(password);
    }

    public bool VerifyPassword(UserPassword password, IStringHashAlgorithm manager, out Error error)
    {
        bool verified = manager.Verify(password.Password, Password);
        if (!verified)
            error = new Error("Пароль неверный.", ErrorCodes.Unauthorized);

        error = Error.None();
        return verified;
    }

    public static HashedUserPassword Random(IStringHashAlgorithm manager)
    {
        return new HashedUserPassword(manager);
    }
}
