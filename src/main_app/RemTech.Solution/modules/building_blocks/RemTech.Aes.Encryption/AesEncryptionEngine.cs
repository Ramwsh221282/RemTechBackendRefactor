using System.Text;

namespace RemTech.Aes.Encryption;

internal sealed class AesEncryptionEngine(string keyText)
{
    private readonly Lazy<byte[]> _keyBytes = new(() => CutStringFor16LengthArrayBytes(keyText));
    private readonly Lazy<byte[]> _ivBytes = new(() => CutStringFor16LengthArrayBytes(keyText));
    
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

    private static byte[] CutStringFor16LengthArrayBytes(string keyText)
    {
        ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(keyText);
        ReadOnlySpan<byte> cutted = bytes[..16];
        return cutted.ToArray();
    }
}