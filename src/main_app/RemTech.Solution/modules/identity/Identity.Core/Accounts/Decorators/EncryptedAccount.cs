using Identity.Core.Accounts.Protocols;

namespace Identity.Core.Accounts.Decorators;

public sealed class EncryptedAccount(ICryptographyProtocol protocol, Account origin) : Account(origin)
{
    public async Task<Account> Account(CancellationToken ct = default)
    {
        string plainPassword = origin.Password;
        string encryptedPassword = await protocol.Encrypt(plainPassword, ct);
        return new EncryptedAccount(protocol, origin.Copy(password: encryptedPassword));
    }
}