using System.Security.Cryptography;

namespace RemTech.Aes.Encryption;

internal sealed class AesDecryptionStream : AesStream
{
    private readonly MemoryStream _ms;
    private readonly CryptoStream _cs;
    private readonly StreamReader _sr;

    public AesDecryptionStream(ICryptoTransform transform) : base(transform)
    {
        _ms = new MemoryStream();
        _cs = new CryptoStream(_ms, transform, CryptoStreamMode.Read);
        _sr = new StreamReader(_cs);
    }

    public AesDecryptionStream(AesDecryptionStream stream, MemoryStream ms) : base(stream._transform)
    {
        _ms = ms;
        _cs = new CryptoStream(_ms, stream._transform, CryptoStreamMode.Read);
        _sr = new StreamReader(_cs);
    }

    public override async Task<string> WorkWithAsync(string plainText)
    {
        await DisposeSelfAsync();
        byte[] textBytes = Convert.FromBase64String(plainText);
        using AesDecryptionStream stream = new(this, new MemoryStream(textBytes));
        return await stream._sr.ReadToEndAsync();
    }

    public override string WorkWith(string plainText)
    {
        DisposeSelf();
        byte[] textBytes = Convert.FromBase64String(plainText);
        using AesDecryptionStream stream = new(this, new MemoryStream(textBytes));
        return stream._sr.ReadToEnd();
    }
    
    public override void Dispose()
    {
        DisposeSelf();
        base.Dispose();
    }

    private async ValueTask DisposeSelfAsync()
    {
        await _ms.DisposeAsync();
        await _cs.DisposeAsync();
        _sr.Dispose();
    }
    
    private void DisposeSelf()
    {
        _ms.Dispose();
        _cs.Dispose();
        _sr.Dispose();
    }
}