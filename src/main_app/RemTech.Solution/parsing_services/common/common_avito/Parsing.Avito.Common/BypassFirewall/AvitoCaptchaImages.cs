namespace Parsing.Avito.Common.BypassFirewall;

public sealed class AvitoCaptchaImages
{
    private readonly List<byte[]> _bytes;

    public AvitoCaptchaImages() => _bytes = new List<byte[]>(2);

    public AvitoCaptchaImages(AvitoCaptchaImages origin, byte[] imageBytes)
    {
        origin._bytes.Add(imageBytes);
        _bytes = origin._bytes;
    }

    public AvitoCaptchaImages With(byte[] bytes) => 
        Amount() == 2 ? this : new AvitoCaptchaImages(this, bytes);

    public int Amount() => _bytes.Count;
    
    public byte[] ReadMin() => 
        Amount() < 2 ? [] : _bytes.MinBy(b => b.Length)!;
    
    public byte[] ReadMax() => 
        Amount() < 2 ? [] : _bytes.MaxBy(b => b.Length)!;
}