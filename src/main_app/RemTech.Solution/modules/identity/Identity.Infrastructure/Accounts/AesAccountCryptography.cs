using Identity.Application.Accounts;
using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Infrastructure.AesEncryption;

namespace Identity.Infrastructure.Accounts;

public sealed class AesAccountCryptography(AesCryptography cryptography) : IAccountEncrypter, IAccountDecrypter
{
    public async Task<IAccount> Encrypt(IAccount value, CancellationToken ct = default)
    {
        IAccountRepresentation representation = value.Represent(AccountRepresentation.Empty());
        IAccountData data = representation.Data;
        string plainPassword = data.Password;
        string encrypted = await cryptography.EncryptText(plainPassword, ct);
        AccountData updated = AccountData.Copy(password: encrypted, data: data);
        return new Account(updated);
    }

    public async Task<IAccount> Decrypt(IAccount value, CancellationToken ct = default)
    {
        IAccountRepresentation representation = value.Represent(AccountRepresentation.Empty());
        IAccountData data = representation.Data;
        string encryptedPassword = data.Password;
        string decrypted = await cryptography.DecryptText(encryptedPassword, ct);
        AccountData updated = AccountData.Copy(password: decrypted, data: data);
        return new Account(updated);
    }
}