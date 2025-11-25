using Identity.Core.Accounts.Protocols;

namespace Identity.Infrastructure.Cryptography;

public sealed class AesCryptography(
    RemTech.SharedKernel.Infrastructure.AesEncryption.AesCryptography cryptography) :
    ICryptographyProtocol
{
    public Task<string> Encrypt(string input, CancellationToken ct) => 
        cryptography.EncryptText(input, ct);

    public Task<string> Decrypt(string input, CancellationToken ct) =>
        cryptography.DecryptText(input, ct);
}