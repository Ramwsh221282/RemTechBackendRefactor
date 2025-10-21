using Identity.Domain.Users.Ports.Passwords;

namespace Identity.Adapter.PasswordManager;

public sealed class BcryptPasswordManager : IPasswordManager
{
    private const int WorkFactor = 12;

    public string Hash(string origin) => BCrypt.Net.BCrypt.HashPassword(origin, WorkFactor);

    public bool Verify(string origin, string hashed) => BCrypt.Net.BCrypt.Verify(origin, hashed);
}
