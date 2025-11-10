using System.Security.Cryptography;

namespace RemTech.Aes.Encryption;

internal sealed class AesDecryptionStream : AesStream
{
    private readonly DisposableResources _disposables = new();
    private readonly MemoryStream _ms;
    private readonly CryptoStream _cs;
    private readonly StreamReader _sr;

    public AesDecryptionStream(ICryptoTransform transform) : base(transform)
    {
        _ms = new MemoryStream();
        _cs = new CryptoStream(_ms, transform, CryptoStreamMode.Read);
        _sr = new StreamReader(_cs);
        _disposables.Add(_sr).Add(_cs).Add(_ms);
    }

    public AesDecryptionStream(AesDecryptionStream stream, MemoryStream ms) : base(stream._transform)
    {
        _disposables = stream._disposables.Disposed();
        _ms = ms;
        _cs = new CryptoStream(_ms, stream._transform, CryptoStreamMode.Read);
        _sr = new StreamReader(_cs);
        _disposables.Add(_sr).Add(_cs).Add(_ms);
    }

    public override async Task<string> WorkWithAsync(string plainText)
    {
        byte[] textBytes = Convert.FromBase64String(plainText);
        using AesDecryptionStream stream = new(this, new MemoryStream(textBytes));
        return await stream._sr.ReadToEndAsync();
    }

    public override string WorkWith(string plainText)
    {
        byte[] textBytes = Convert.FromBase64String(plainText);
        using AesDecryptionStream stream = new(this, new MemoryStream(textBytes));
        return stream._sr.ReadToEnd();
    }

    public override void Dispose()
    {
        _ms.Dispose();
        _cs.Dispose();
        _sr.Dispose();
        base.Dispose();
    }
}