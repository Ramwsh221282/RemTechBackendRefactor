using System.Security.Cryptography;

namespace RemTech.Aes.Encryption;

internal abstract class AesStream : IDisposable
{
    protected readonly ICryptoTransform _transform;

    public AesStream(ICryptoTransform transform)
    {
        _transform = transform;
    }

    public virtual void Dispose()
    {
        _transform.Dispose();
    }

    public abstract Task<string> WorkWithAsync(string plainText);
    public abstract string WorkWith(string plainText);
}