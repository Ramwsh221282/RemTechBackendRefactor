namespace Identity.Core.Accounts.Protocols;

public interface ICryptographyProtocol
{
    Task<string> Encrypt(string input, CancellationToken ct);
    Task<string> Decrypt(string input, CancellationToken ct);
}