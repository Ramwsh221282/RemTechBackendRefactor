using System.Security.Cryptography;

namespace RemTech.Aes.Encryption;

internal sealed class AesDecryptionScope : AesScope
{
    private readonly ICryptoTransform _decryptor;

    public AesDecryptionScope(byte[] keyBytes, byte[] ivBytes) : base(keyBytes, ivBytes)
    {
        _decryptor = Aes.CreateDecryptor();
    }

    public override AesStream ProvideStream()
    {
        return new AesDecryptionStream(_decryptor);
    }

    public override void Dispose()
    {
        base.Dispose();
        _decryptor.Dispose();
    }
}