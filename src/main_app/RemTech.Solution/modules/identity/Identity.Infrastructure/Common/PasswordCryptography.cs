using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using RemTech.SharedKernel.Infrastructure.AesEncryption;

namespace Identity.Infrastructure.Common;

public sealed class PasswordCryptography(AesCryptography cryptography) : IPasswordCryptography
{
    private AesCryptography Cryptography { get; } = cryptography;
    
    public async Task<AccountPassword> Encrypt(AccountPassword password, CancellationToken ct = default)
    {
        string encryptedPasswordString = await Cryptography.EncryptText(password.Value, ct);
        return Recreate(encryptedPasswordString);
    }

    public async Task<AccountPassword> Decrypt(AccountPassword password, CancellationToken ct = default)
    {
        string decryptedPasswordString = await Cryptography.DecryptText(password.Value, ct);
        return Recreate(decryptedPasswordString);
    }

    private AccountPassword Recreate(string modifiedPasswordString)
    {
        return AccountPassword.Create(modifiedPasswordString);
    }
}