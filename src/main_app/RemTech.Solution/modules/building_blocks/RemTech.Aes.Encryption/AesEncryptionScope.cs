using System.Security.Cryptography;

namespace RemTech.Aes.Encryption;

internal sealed class AesEncryptionScope : AesScope
{
    private readonly ICryptoTransform _encryptor;

    public AesEncryptionScope(byte[] keyBytes, byte[] ivBytes) : base(keyBytes, ivBytes)
    {
        _encryptor = Aes.CreateEncryptor(keyBytes, ivBytes);
    }

    public override AesStream ProvideStream()
    {
        return new AesEncryptionStream(_encryptor);
    }

    public override void Dispose()
    {
        base.Dispose();
        _encryptor.Dispose();
    }
}