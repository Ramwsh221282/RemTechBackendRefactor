using Identity.Application.Accounts;
using Identity.Contracts.Accounts;

namespace Identity.Infrastructure.Accounts;

public sealed class AesAccountCryptography(AesCryptography cryptography) : IAccountCryptography
{
    public async Task<IAccount> Encrypt(IAccount value, CancellationToken ct = default)
    {
        AccountData data = value.Represent();
        string plainPassword = data.Password;
        string encrypted = await cryptography.EncryptText(plainPassword, ct);
        AccountData updated = AccountData.Copy(password: encrypted, data: data);
        return new Account(updated);   
    }

    public async Task<IAccount> Decrypt(IAccount value, CancellationToken ct = default)
    {
        AccountData data = value.Represent();
        string encryptedPassword = data.Password;
        string decrypted = await cryptography.DecryptText(encryptedPassword, ct);
        AccountData updated = AccountData.Copy(password: decrypted, data: data);
        return new Account(updated);
    }
}