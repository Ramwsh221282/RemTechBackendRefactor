namespace Identity.Domain.Users.Ports.Passwords;

public interface IPasswordManager
{
    public string Hash(string origin);
    public bool Verify(string origin, string hashed);
}
