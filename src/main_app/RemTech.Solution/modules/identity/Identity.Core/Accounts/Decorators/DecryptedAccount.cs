using Identity.Core.Accounts.Protocols;

namespace Identity.Core.Accounts.Decorators;

public sealed class DecryptedAccount(ICryptographyProtocol protocol, Account origin) : Account(origin)
{
    public async Task<Account> Decrypt(CancellationToken ct = default)
    {
        string encryptedPassword = origin.Password;
        string decryptedPassword = await protocol.Decrypt(encryptedPassword, ct);
        return new DecryptedAccount(protocol, origin.Copy(password: decryptedPassword));
    }
}