namespace Identity.Domain.Users.Ports.Passwords;

public interface IStringHashAlgorithm
{
    public string Hash(string origin);
    public bool Verify(string origin, string hashed);
}
