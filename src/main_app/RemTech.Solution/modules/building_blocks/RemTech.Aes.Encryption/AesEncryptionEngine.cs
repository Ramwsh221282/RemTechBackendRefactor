using System.Security.Cryptography;
using System.Text;

namespace RemTech.Aes.Encryption;

internal sealed class AesEncryptionEngine(string keyText)
{
    private readonly Lazy<byte[]> _keyBytes = new(() => Encoding.UTF8.GetBytes(keyText));
    private readonly Lazy<byte[]> _ivBytes = new(() => CreateIvBytes(keyText));
    private byte[] Key => _keyBytes.Value;
    private byte[] Iv => _ivBytes.Value;

    public AesScope ProvideDecryptor()
    {
        return new AesDecryptionScope(Key, Iv);
    }

    public AesScope ProvideEncryptor()
    {
        return new AesEncryptionScope(Key, Iv);
    }

    private static byte[] CreateIvBytes(string keyText)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(keyText)).AsSpan(..16).ToArray();
    }
}