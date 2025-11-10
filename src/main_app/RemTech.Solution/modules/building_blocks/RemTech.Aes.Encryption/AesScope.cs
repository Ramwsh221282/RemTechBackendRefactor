namespace RemTech.Aes.Encryption;

internal abstract class AesScope : IDisposable
{
    protected readonly System.Security.Cryptography.Aes Aes;

    public AesScope(byte[] keyBytes, byte[] ivBytes)
    {
        Aes = System.Security.Cryptography.Aes.Create();
        Aes.IV = ivBytes;
        Aes.Key = keyBytes;
    }

    public abstract AesStream ProvideStream();

    public virtual void Dispose()
    {
        Aes.Dispose();
    }
}