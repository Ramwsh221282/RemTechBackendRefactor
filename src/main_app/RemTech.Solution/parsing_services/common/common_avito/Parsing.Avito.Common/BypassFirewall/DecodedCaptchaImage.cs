using OpenCvSharp;

namespace Parsing.Avito.Common.BypassFirewall;

public sealed class DecodedCaptchaImage : IDisposable
{
    private readonly byte[] _bytes;
    private readonly Mat _mat;

    public DecodedCaptchaImage(byte[] encodedBytes)
    {
        _bytes = encodedBytes;
        _mat = _bytes.Length == 0 ? new Mat() : Cv2.ImDecode(encodedBytes, ImreadModes.Color);
    }
    
    public void Dispose() => _mat.Dispose();

    public Mat Read() => _bytes.Length == 0
        ? throw new ArgumentException("Decoded captcha image requires non zero length byte array.")
        : _mat;
}