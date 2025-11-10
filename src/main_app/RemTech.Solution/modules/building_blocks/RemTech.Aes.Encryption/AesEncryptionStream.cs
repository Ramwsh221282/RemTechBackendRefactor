using System.Security.Cryptography;

namespace RemTech.Aes.Encryption;

internal sealed class AesEncryptionStream : AesStream
{
    private readonly DisposableResources _disposables = new();
    private readonly MemoryStream _ms;
    private readonly StreamWriter _sw;
    private readonly CryptoStream _cs;

    public AesEncryptionStream(ICryptoTransform transform) : base(transform)
    {
        _ms = new MemoryStream();
        _cs = new CryptoStream(_ms, transform, CryptoStreamMode.Write);
        _sw = new StreamWriter(_cs);
        _disposables = _disposables.Add(_sw).Add(_cs).Add(_ms);
    }

    public override async Task<string> WorkWithAsync(string plainText)
    {
        await _sw.WriteAsync(plainText);
        _sw.Close();
        return Convert.ToBase64String(_ms.ToArray());
    }

    public override string WorkWith(string plainText)
    {
        _sw.Write(plainText);
        _sw.Close();
        return Convert.ToBase64String(_ms.ToArray());
    }

    public override void Dispose()
    {
        _disposables.Disposed();
        base.Dispose();
    }
}